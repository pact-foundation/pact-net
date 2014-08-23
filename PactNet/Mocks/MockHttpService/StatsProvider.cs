using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class StatsProvider : IStatsProvider
    {
        private readonly List<Stat> _stats = new List<Stat>();
        public IEnumerable<Stat> Stats { get { return _stats; } }

        public StatsProvider()
        {
            
        }
 
        public void AddStat(Stat stat)
        {
            _stats.Add(stat);
        }

        public void Clear()
        {
            _stats.Clear(); 
        }
    }
}