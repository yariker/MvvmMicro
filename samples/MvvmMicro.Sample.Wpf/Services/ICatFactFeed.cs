using System;
using System.Threading;
using System.Threading.Tasks;
using MvvmMicro.Sample.Wpf.Model;

namespace MvvmMicro.Sample.Wpf.Services
{
    public interface ICatFactFeed
    {
        Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken = default);
    }
}