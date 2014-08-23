using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IStatsProvider
    {
        IEnumerable<Stat> Stats { get; }
        void AddStat(Stat stat);
        void Clear();
    }
}