﻿namespace Owin.RequiresHttps
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RequiresHttps
    {
        private readonly Func<IDictionary<string, object>, Task> nextFunc;
        private readonly RequiresHttpsOptions options;

        public RequiresHttps(Func<IDictionary<string, object>, Task> nextFunc, RequiresHttpsOptions options)
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
            return Task.FromResult(0);
        }
    }
}
