using MvvmMicro.Sample.Wpf.Model;
using MvvmMicro.Sample.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MvvmMicro.Sample.Wpf.Design;

public class CatFactFeed : ICatFactFeed
{
    public Task<List<CatFact>> GetFactsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<List<CatFact>>(
        [
            new CatFact
            {
                Picture = new BitmapImage(new Uri("https://purr.objects-us-east-1.dream.io/i/vLgexh.jpg")),
                Fact = "A female cat will be pregnant for approximately 9 weeks - between 62 and 65 days from conception to delivery.",
            },
            new CatFact
            {
                Picture = new BitmapImage(new Uri("https://purr.objects-us-east-1.dream.io/i/Eu8F6.jpg")),
                Fact = "It has been scientifically proven that stroking a cat can lower one's blood pressure.",
            },
            new CatFact
            {
                Picture = new BitmapImage(new Uri("https://purr.objects-us-east-1.dream.io/i/hgyw6Tc.jpg")),
                Fact = "In an average year, cat owners in the United States spend over $2 billion on cat food.",
            },
        ]);
    }
}
