# ASP.NET Core output caching middleware

[![Build status](https://ci.appveyor.com/api/projects/status/rqp3tneiy0bi1697?svg=true)](https://ci.appveyor.com/project/madskristensen/webessentials-aspnetcore-outputcaching)
[![NuGet](https://img.shields.io/nuget/v/WebEssentials.AspNetCore.OutputCaching.svg)](https://nuget.org/packages/WebEssentials.AspNetCore.OutputCaching/)

Server-side caching middleware for ASP.NET 2.0

Start by registering the service it in `Startup.cs` like so:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddOutputCaching();
}
```

...and then register the middleware just before the call to `app.UseMvc(...)` like so:

```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseOutputCaching();
    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```