using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MvvmMicro.Sample.Wpf.Model;

namespace MvvmMicro.Sample.Wpf.Services
{
    public sealed class CatFactFeed : ICatFactFeed, IDisposable
    {
        private static readonly Uri RandomPictureUri = new("https://cataas.com/cat");
        private readonly HttpClient _client = new() { MaxResponseContentBufferSize = 5 * 1024 * 1024 };

        static CatFactFeed()
        {
            ServicePointManager.DefaultConnectionLimit = 10;
        }

        public async Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken)
        {
            // Download facts.
            var uri = $"https://cat-fact.herokuapp.com/facts/random?amount={amount}";
            var facts = await _client.GetFromJsonAsync<Fact[]>(uri, cancellationToken) ?? Array.Empty<Fact>();

            // Download pictures (in parallel).
            var pictures = facts.Select(_ => _client.GetStreamAsync(RandomPictureUri, cancellationToken)).ToArray();
            await Task.WhenAll(pictures);

            // Assign pictures.
            for (var i = 0; i < facts.Length; i++)
            {
                facts[i].Picture = CreateImageSource(await pictures[i]);
            }

            return facts;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static ImageSource CreateImageSource(Stream stream)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
    }
}