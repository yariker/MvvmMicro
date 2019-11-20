using System;
using System.Threading;
using System.Threading.Tasks;
using PlaidSoft.MvvmMicro.Sample.NetFx.Model;

namespace PlaidSoft.MvvmMicro.Sample.NetFx.Services
{
    public interface ICatFactFeed
    {
        Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken = default);
    }
}