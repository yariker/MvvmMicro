using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MvvmMicro.Sample.Wpf.Model;

namespace MvvmMicro.Sample.Wpf.Services
{
    public sealed class CatFactFeed : ICatFactFeed, IDisposable
    {
        private static readonly Uri RandomPictureUri = new Uri("https://cataas.com/cat");
        private readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 5 * 1024 * 1024 };

        public async Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken)
        {
            CancellationTokenRegistration cancellationRegistration =
                cancellationToken.CanBeCanceled
                    ? cancellationToken.Register(() => _client.CancelPendingRequests())
                    : default;

            using (cancellationRegistration)
            {
                // Download facts.
                string uri = $"https://cat-fact.herokuapp.com/facts/random?amount={amount}";
                Fact[] facts = await _client.GetAsync<Fact[]>(uri);

                // Download pictures (in parallel).
                ServicePointManager.DefaultConnectionLimit = facts.Length;
                Task<Stream>[] pictures = facts.Select(x => _client.GetStreamAsync(RandomPictureUri)).ToArray();
                await Task.WhenAll(pictures);

                // Assign pictures.
                for (int i = 0; i < facts.Length; i++)
                {
                    facts[i].Picture = CreateImageSource(await pictures[i]);
                }

                return facts;
            }
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