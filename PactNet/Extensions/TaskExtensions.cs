using System.Threading.Tasks;

namespace PactNet.Extensions
{
    public static class TaskExtensions
    {
        public static T RunSync<T>(this Task<T> task)
        {
            return Task.Factory.StartNew(() => task.Result).Result;
        }
    }
}
