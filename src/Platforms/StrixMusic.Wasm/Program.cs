namespace StrixMusic.Wasm
{
    /// <summary>
    /// From the Uno template
    /// </summary>
    public class Program
    {
        private static App? _app;

        /// <summary>
        /// From the Uno template
        /// </summary>
        /// <param name="args">From the Uno template</param>
        /// <returns>From the Uno template</returns>
        public static int Main(string[] args)
        {
            Windows.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;
        }
    }
}
