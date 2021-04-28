using System;
using System.Diagnostics;
using System.Threading.Tasks;
using log4net;

namespace ClusterClient.Clients
{
    public class RoundRobinClusterClient : ClusterClientBase
    {
        public RoundRobinClusterClient(string[] replicaAddresses) : base(replicaAddresses)
        {
        }

        protected override ILog Log => LogManager.GetLogger(typeof(RoundRobinClusterClient));

        public override async Task<string> ProcessRequestAsync(string query, TimeSpan timeout)
        {
            var sw = new Stopwatch();
            var delta = ReplicaAddresses.Length;
            foreach (var uri in ReplicaAddresses)
            {
                var webRequest = CreateRequest(uri + "?query=" + query);

                Log.InfoFormat($"Processing {webRequest.RequestUri}");
                
                var resultTask = ProcessRequestAsync(webRequest);
                
                sw.Start();
                
                await Task.WhenAny(resultTask, Task.Delay(timeout.Divide(delta)));
                
                sw.Stop();
                timeout -= sw.Elapsed;
                sw.Reset();
                
                delta--;
                
                if (resultTask.IsCompletedSuccessfully)
                    return resultTask.Result;
            }

            throw new TimeoutException();
        }

        private TimeSpan GetSoftTimeout(TimeSpan timeSpan)
        {
            return timeSpan.Divide(ReplicaAddresses.Length);
        }
    }
}