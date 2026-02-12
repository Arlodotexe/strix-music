// Audio playback via blob URLs for WASM.
// Uno's MediaSource.CreateFromStream and MediaPlayer are not fully implemented on WASM.
var BlobInterop = {
    _audio: null,
    _onEndedCallback: null,
    _onTimeUpdateCallback: null,

    createBlobUrl: function (base64Data, contentType) {
        var byteCharacters = atob(base64Data);
        var byteNumbers = new Uint8Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        var blob = new Blob([byteNumbers], { type: contentType });
        return URL.createObjectURL(blob);
    },

    play: function (blobUrl) {
        if (BlobInterop._audio) {
            BlobInterop._audio.pause();
            BlobInterop._audio.removeAttribute('src');
            BlobInterop._audio.load();
        }
        BlobInterop._audio = new Audio(blobUrl);
        BlobInterop._audio.onended = function () {
            if (BlobInterop._onEndedCallback) {
                BlobInterop._onEndedCallback();
            }
        };
        BlobInterop._audio.play();
        return "ok";
    },

    pause: function () {
        if (BlobInterop._audio) BlobInterop._audio.pause();
        return "ok";
    },

    resume: function () {
        if (BlobInterop._audio) BlobInterop._audio.play();
        return "ok";
    },

    stop: function () {
        if (BlobInterop._audio) {
            BlobInterop._audio.pause();
            BlobInterop._audio.currentTime = 0;
        }
        return "ok";
    },

    setVolume: function (volume) {
        if (BlobInterop._audio) BlobInterop._audio.volume = volume;
        return "ok";
    },

    getPosition: function () {
        if (BlobInterop._audio) return BlobInterop._audio.currentTime.toString();
        return "0";
    },

    getDuration: function () {
        if (BlobInterop._audio) return BlobInterop._audio.duration.toString();
        return "0";
    },

    setPosition: function (seconds) {
        if (BlobInterop._audio) BlobInterop._audio.currentTime = seconds;
        return "ok";
    },

    getState: function () {
        if (!BlobInterop._audio) return "none";
        if (BlobInterop._audio.paused) return "paused";
        return "playing";
    },

    revokeBlobUrl: function (url) {
        URL.revokeObjectURL(url);
    }
};
