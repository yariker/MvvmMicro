using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MvvmMicro.Sample.Wpf.Model;

namespace MvvmMicro.Sample.Wpf.Services;

public interface ICatFactFeed
{
    Task<List<CatFact>> GetFactsAsync(CancellationToken cancellationToken = default);
}
