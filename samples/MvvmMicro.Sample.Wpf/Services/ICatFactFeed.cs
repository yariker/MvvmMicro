using System;
using System.Threading;
using System.Threading.Tasks;
using Takesoft.MvvmMicro.Sample.NetFx.Model;

namespace Takesoft.MvvmMicro.Sample.NetFx.Services
{
    public interface ICatFactFeed
    {
        Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken = default);
    }
}