using System;
using System.Threading.Tasks;

namespace PactNet.Extensions
{
    public static class TaskExtensions
    {
        public static void RunSync(this Task task)
        {
            task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public static T RunSync<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
