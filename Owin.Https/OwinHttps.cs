namespace Owin.Https
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class OwinHttps
    {
        private readonly Func<IDictionary<string, object>, Task> nextFunc;
        private readonly OwinHttpsOptions options;

        public OwinHttps(Func<IDictionary<string, object>, Task> nextFunc, OwinHttpsOptions options)
        {
            this.nextFunc = nextFunc;
            this.options = options;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var scheme = (string)environment["owin.RequestScheme"];
            
            if (scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) 
                return nextFunc(environment);
            
            if (options != null && !string.IsNullOrWhiteSpace(options.RedirectToHttpsPath))
            {
                var headers = environment["owin.ResponseHeaders"] as IDictionary<string, string[]>;
                headers = headers ?? new Dictionary<string, string[]>();
                headers.Add("Location", new[] { options.RedirectToHttpsPath });
                environment["owin.ResponseStatusCode"] = 302;
                return ReturnCompletedTask();
            }
            
            environment["owin.ResponseStatusCode"] = 401;
            return ReturnCompletedTask();
        }

        private Task ReturnCompletedTask()
        {
            var tsc = new TaskCompletionSource<int>();
            tsc.TrySetResult(0);
            return tsc.Task;
        }
    }
}
