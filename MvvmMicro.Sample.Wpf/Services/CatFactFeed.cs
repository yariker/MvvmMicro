using System;
using System.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PlaidSoft.MvvmMicro.Sample.NetFx.Model;

namespace PlaidSoft.MvvmMicro.Sample.NetFx.Services
{
    public sealed class CatFactFeed : ICatFactFeed, IDisposable
    {
        private static readonly TimeSpan MinLoadTime = TimeSpan.FromSeconds(2);
        private readonly HttpClient _client = new HttpClient();

        public async Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            // Download facts.
            string uri = $"https://cat-fact.herokuapp.com/facts/random?amount={amount}";
            var facts = await _client.GetAsync<Fact[]>(uri, cancellationToken).ConfigureAwait(false);

            // Download random pictures.
            var tasks = facts.Select(_ => _client.GetAsync<Picture>("https://aws.random.cat/meow", cancellationToken)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (int i = 0; i < facts.Length; i++)
            {
                facts[i].PictureUri = tasks[i].Result.File;
            }

            // Artificial delay for demo purposes.
            stopwatch.Stop();
            if (stopwatch.Elapsed < MinLoadTime)
            {
                await Task.Delay(MinLoadTime - stopwatch.Elapsed, cancellationToken).ConfigureAwait(false);
            }

            return facts;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}