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

namespace MvvmMicro.Sample.Wpf.Services;

public sealed class CatFactFeed : ICatFactFeed, IDisposable
{
    private readonly HttpClient _client = new()
    {
        MaxResponseContentBufferSize = 5 * 1024 * 1024,
        Timeout = TimeSpan.FromSeconds(30),
    };

    static CatFactFeed()
    {
        ServicePointManager.DefaultConnectionLimit = 5;
    }

    public async Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken)
    {
        // Download facts.
        var uri = $"https://cat-fact.herokuapp.com/facts/random?amount={amount}";
        var facts = await _client.GetFromJsonAsync<Fact[]>(uri, cancellationToken) ?? Array.Empty<Fact>();

        // Download pictures (in parallel).
        var pictures = facts.Select(_ => GetPictureAsync(cancellationToken)).ToArray();
        await Task.WhenAll(pictures);

        // Assign pictures.
        for (var i = 0; i < facts.Length; i++)
        {
            facts[i].Picture = CreateImageSource(await pictures[i]);
        }

        return facts;
    }

    private async Task<Stream> GetPictureAsync(CancellationToken cancellationToken)
    {
        var picture = await _client.GetFromJsonAsync<Picture>("https://aws.random.cat/meow", cancellationToken)
                                   .ConfigureAwait(false);

        var stream = await _client.GetStreamAsync(picture.File, cancellationToken)
                                  .ConfigureAwait(false);

        return stream;
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
