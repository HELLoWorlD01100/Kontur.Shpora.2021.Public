using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClusterClient.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task<(bool Ok, T TaskResult)> TryGetFirstSuccess<T>(this IEnumerable<Task<T>> source)
        {
            var tasks = source.ToList();
            
            while (tasks.Count > 0)
            {
                var currentTask = await Task.WhenAny(tasks);
                if (!currentTask.IsCompletedSuccessfully)
                    tasks.Remove(currentTask);
                else
                    return (true, await currentTask);
            }

            return default;
        }
        
        public static T[] Shuffle<T>(this IEnumerable<T> list)
        {
            var rand = new Random();
            var result = list.ToArray();
            for (var i = result.Length - 1; i >= 1; i--)
            {
                var j = rand.Next(i + 1);
 
                var tmp = result[j];
                result[j] = result[i];
                result[i] = tmp;
            }

            return result;
        }
    }
}