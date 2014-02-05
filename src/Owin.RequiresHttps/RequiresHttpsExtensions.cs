namespace Owin.RequiresHttps
{
    public static class RequiresHttpsExtensions
    {
        public static IAppBuilder RequiresHttps(this IAppBuilder app, RequiresHttpsOptions options)
        {
            return app.Use<RequiresHttps>(options);
        }
    }
}
