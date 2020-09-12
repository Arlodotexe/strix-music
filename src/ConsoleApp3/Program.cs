using StrixMusic.Core.MusicBrainz;
using System;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LoadInfo().Wait();
        }
        async static Task LoadInfo()
        {
            var core = new MusicBrainzCore("10");
            await core.InitAsync();
            await core.Library.PopulateArtistsAsync(10, 20);
        }
    }
}
