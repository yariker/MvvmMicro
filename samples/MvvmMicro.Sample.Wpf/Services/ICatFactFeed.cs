using System;
using System.Threading;
using System.Threading.Tasks;
using Takesoft.MvvmMicro.Sample.Wpf.Model;

namespace Takesoft.MvvmMicro.Sample.Wpf.Services
{
    public interface ICatFactFeed
    {
        Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken = default);
    }
}