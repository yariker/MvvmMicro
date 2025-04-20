using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MvvmMicro.Sample.Wpf.Model;

namespace MvvmMicro.Sample.Wpf.Services;

public sealed class CatFactFeed(IHttpClientFactory httpClientFactory) : ICatFactFeed, IDisposable
{
    public const string HttpClientName = "cat-fact-feed";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private readonly HttpClient _client = httpClientFactory.CreateClient(HttpClientName);
    private CatFactList _page;

    static CatFactFeed()
    {
        ServicePointManager.DefaultConnectionLimit = 5;
    }

    public async Task<List<CatFact>> GetFactsAsync(CancellationToken cancellationToken)
    {
        // No more facts =(
        if (_page != null && _page.CurrentPage == _page.LastPage)
        {
            return [];
        }

        // Download facts.
        var page = _page != null ? _page.CurrentPage + 1 : 1;
        var uri = $"https://catfact.ninja/facts?page={page}";
        _page = await _client.GetFromJsonAsync<CatFactList>(uri, JsonOptions, cancellationToken);

        // Download pictures (in parallel).
        var pictures = _page.Data.Select(_ => GetPictureAsync(cancellationToken)).ToArray();
        await Task.WhenAll(pictures);

        // Assign pictures.
        for (var i = 0; i < _page.Data.Count; i++)
        {
            _page.Data[i].Picture = CreateImageSource(await pictures[i]);
        }

        return _page.Data;
    }

    private async Task<Stream> GetPictureAsync(CancellationToken cancellationToken)
    {
        var stream = await _client.GetStreamAsync("https://cataas.com/cat", cancellationToken)
                                  .ConfigureAwait(false);
        return stream;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private static BitmapImage CreateImageSource(Stream stream)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        return image;
    }
}
