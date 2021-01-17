using OwlCore.Provisos;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace StrixMusic.Sdk.Uno.Services.MediaPlayback
{
    /// <summary>
    /// An <see cref="IMemoryBuffer"/> with direct access.
    /// </summary>
    internal unsafe interface IMemoryBufferByteAccess
    {
        /// <summary>
        /// Get access to the buffer bytes.
        /// </summary>
        /// <param name="buffer">The buffer as an out parameter.</param>
        /// <param name="capacity">The capacity of the out buffer.</param>
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    /// <summary>
    /// A class that uses an <see cref="AudioGraph"/> to extract PCM data out of a <see cref="MediaSource"/>
    /// </summary>
    internal class AudioGraphLeech : IAsyncInit
    {
        private AudioGraph? _graph;
        private AudioFrameOutputNode? _outNode;
        private MediaSourceAudioInputNode? _inNode; // TODO: Attach multiple nodes per core, instead of creating multiple graphs
        private int _quantum = 0;

        /// <summary>
        /// Raised when a quantum of data is processed. 
        /// </summary>
        public event EventHandler<float[]>? QuantumProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioGraphLeech"/> class.
        /// </summary>
        public AudioGraphLeech()
        {
        }

        /// <inheritdoc />
        public bool IsInitialized { get; set; }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            await CreateGraph();
            CreateFrameOutNode();

            IsInitialized = true;
        }

        /// <summary>
        /// Creates a <see cref="MediaSourceAudioInputNode"/> bound to the Graph, to extract PCM with.
        /// </summary>
        /// <param name="mediaSource">The <see cref="MediaSource"/> to extract from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AttachMediaSource(MediaSource mediaSource)
        {
            if (_graph is null)
            {
                return;
            }

            CreateMediaSourceAudioInputNodeResult result = await _graph.CreateMediaSourceAudioInputNodeAsync(mediaSource);

            if (result.Status != MediaSourceAudioInputNodeCreationStatus.Success)
            {
                // Cannot create node
                return;
            }

            _inNode = result.Node;
        }

        /// <summary>
        /// Begin extracting PCM data.
        /// </summary>
        public void Begin()
        {
            if (_inNode is null ||
                _outNode is null ||
                _graph is null)
                return;

            _inNode.Start();
            _outNode.Start();
            _graph.Start();
        }

        /// <summary>
        /// Pause extracting PCM data.
        /// </summary>
        public void Stop()
        {
            if (_inNode is null ||
                _outNode is null ||
                _graph is null)
                return;

            _inNode.Stop();
            _outNode.Stop();
            _graph.Stop();
        }

        private async Task CreateGraph()
        {
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(graphsettings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            _graph = result.Graph;
        }

        private void CreateFrameOutNode()
        {
            if (_graph is null)
            {
                return;
            }

            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
            nodesettings.EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32);
            nodesettings.DesiredSamplesPerQuantum = 960;
            nodesettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            _outNode = _graph.CreateFrameOutputNode(_graph.EncodingProperties);
            _quantum = 0;
            _graph.QuantumStarted += QuantumStarted;
        }

        private unsafe void QuantumStarted(AudioGraph sender, object args)
        {
            if (_outNode is null)
            {
                return;
            }

            // Only read even quantum to read one channel.
            if (++_quantum % 2 == 0)
            {
                AudioFrame frame = _outNode.GetFrame();
                float[] dataInFloats;
                using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
                using (IMemoryBufferReference reference = buffer.CreateReference())
                {
                    // Get the buffer from the AudioFrame
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                    float* dataInFloat = (float*)dataInBytes;
                    dataInFloats = new float[capacityInBytes / sizeof(float)];

                    for (int i = 0; i < capacityInBytes / sizeof(float); i++)
                    {
                        dataInFloats[i] = dataInFloat[i];
                    }
                }
            }
        }
    }
}
