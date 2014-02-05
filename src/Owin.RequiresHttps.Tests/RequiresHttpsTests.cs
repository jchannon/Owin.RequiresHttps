namespace Owin.RequiresHttps.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class OwinHttpsTests
    {
        [Fact]
        public void Should_Execute_Next_If_Https()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc());
            var environment = new Dictionary<string, object> { { "owin.RequestScheme", "https" } };

            //When
            var task = owinhttps.Invoke(environment);

            //Then
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(123, ((Task<int>)task).Result);
        }

        [Fact]
        public void Should_Return_401_If_Http()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc());
            var environment = new Dictionary<string, object> { { "owin.RequestScheme", "http" } };

            //When
            owinhttps.Invoke(environment);

            //Then
            Assert.Equal(401, environment["owin.ResponseStatusCode"]);
        }

        [Fact]
        public void Should_Return_Completed_Task_If_Http()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc());
            var environment = new Dictionary<string, object> { { "owin.RequestScheme", "http" } };

            //When
            var task = owinhttps.Invoke(environment);

            //Then
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(0, ((Task<int>)task).Result);
        }

        [Fact]
        public void Should_Return_302_If_Options_Provided()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc(),
                new OwinHttpsOptions() { RedirectToHttpsPath = "http://www.google.co.uk" });

            var environment = new Dictionary<string, object>
            {
                {"owin.RequestScheme", "http"},
                {"owin.ResponseHeaders", new Dictionary<string, string[]>()}
            };

            //When
            owinhttps.Invoke(environment);

            //Then
            var headers = environment["owin.ResponseHeaders"] as IDictionary<string, string[]>;

            Assert.Equal("http://www.google.co.uk", headers["Location"].First());
            Assert.Equal(302, environment["owin.ResponseStatusCode"]);
        }

        [Fact]
        public void Should_Return_Completed_Task_If_Http_And_Options_Provided()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc(),
                new OwinHttpsOptions() {RedirectToHttpsPath = "http://www.google.co.uk"});

            var environment = new Dictionary<string, object>
            {
                {"owin.RequestScheme", "http"},
                {"owin.ResponseHeaders", new Dictionary<string, string[]>()}
            };

            //When
            var task = owinhttps.Invoke(environment);

            //Then
            Assert.Equal(true, task.IsCompleted);
            Assert.Equal(0, ((Task<int>)task).Result);
        }

        [Fact]
        public void Should_Return_401_If_Options_RedirectURL_Not_Specified()
        {
            //Given
            var owinhttps = GetOwinHttps(GetNextFunc(),
                new OwinHttpsOptions() { RedirectToHttpsPath = "" });

            var environment = new Dictionary<string, object>
            {
                {"owin.RequestScheme", "http"},
                {"owin.ResponseHeaders", new Dictionary<string, string[]>()}
            };

            //When
            owinhttps.Invoke(environment);

            //Then
            Assert.Equal(401, environment["owin.ResponseStatusCode"]);
        }

        public Func<IDictionary<string, object>, Task> GetNextFunc()
        {
            return objects =>
            {
                var tcs = new TaskCompletionSource<int>();
                tcs.TrySetResult(123);
                return tcs.Task;
            };
        }

        public RequiresHttps GetOwinHttps(Func<IDictionary<string, object>, Task> nextFunc, OwinHttpsOptions options = null)
        {
            return new RequiresHttps(nextFunc, options);
        }
    }
}
