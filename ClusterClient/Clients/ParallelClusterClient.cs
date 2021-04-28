using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClusterClient.Extensions;
using log4net;

namespace ClusterClient.Clients
{
    public class ParallelClusterClient : ClusterClientBase
    {
        public ParallelClusterClient(string[] replicaAddresses) : base(replicaAddresses)
        {
        }

        public override async Task<string> ProcessRequestAsync(string query, TimeSpan timeout)
        {
            var tasks = ReplicaAddresses.Select(async uri =>
            {
                var webRequest = CreateRequest(uri + "?query=" + query);
            
                Log.InfoFormat($"Processing {webRequest.RequestUri}");

                var resultTask = ProcessRequestAsync(webRequest);

                await Task.WhenAny(resultTask, Task.Delay(timeout));
                if (!resultTask.IsCompleted)
                    throw new TimeoutException();
                
                return resultTask.Result;
            });

            var (ok, taskResult) = await tasks.TryGetFirstSuccess();
            if (ok)
                return taskResult;
            else
                throw new TimeoutException();
        }
        
        protected override ILog Log => LogManager.GetLogger(typeof(ParallelClusterClient));
    }
    
}
