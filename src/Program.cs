
using Microsoft.AspNetCore;
using Microsoft.Extensions.FileProviders;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace revers;

internal static class Program
{
    private static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("settings.json")
            .Build();

        if (!int.TryParse(configuration["options:port"], out var portNumber))
        {
            portNumber = 8080;
        }

        var builder = WebHost.CreateDefaultBuilder(args);
        builder.UseConfiguration(configuration);
        builder.UseKestrel(options => options.ListenLocalhost(portNumber));
        builder.UseStartup<Startup>();

        var app = builder.Build();

        app.Run();
    }
}

internal class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var reverseProxyMappings = GetReverseProxyMappings().ToList();

        if (reverseProxyMappings.Count != 0)
        {
            services.AddReverseProxy(reverseProxyMappings);
        }
    }

    public void Configure(IApplicationBuilder app)
    {
        var staticFileMappings = GetStaticFileMappings();

        if (staticFileMappings.Any())
        {
            app.UseDefaultFiles();

            foreach (var (requestPath, localDirectory) in staticFileMappings)
            {
                app.MapStaticFiles(requestPath, localDirectory);
            }
        }

        var reverseProxyMappings = GetReverseProxyMappings();

        if (reverseProxyMappings.Any())
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });
        }
    }

    private IEnumerable<(string, Uri)> GetReverseProxyMappings()
    {
        foreach (var (key, value) in Configuration.AsEnumerable())
        {
            if (IsReverseProxyMapping(key, value))
            {
                yield return (key, new Uri(value!));
            }
        }
    }

    private static bool IsReverseProxyMapping(string maybeApath, string? maybeAuri)
        => maybeApath.StartsWith('/')
            && Uri.TryCreate(maybeAuri, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    private IEnumerable<(string, DirectoryInfo)> GetStaticFileMappings()
    {
        foreach (var (key, value) in Configuration.AsEnumerable())
        {
            if (IsStaticFileMapping(key, value))
            {
                yield return (key, new DirectoryInfo(value!));
            }
        }
    }

    private static bool IsStaticFileMapping(string maybeApath, string? maybeAdirectory)
        => maybeApath.StartsWith('/') && Directory.Exists(maybeAdirectory);
}

internal static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder MapStaticFiles(this IApplicationBuilder app, string requestPath, DirectoryInfo localDirectory)
    {
        return app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(localDirectory.FullName),
            RequestPath = requestPath
        });
    }
}

internal static class IServiceCollectionExtensions
{
    public static IReverseProxyBuilder AddReverseProxy(this IServiceCollection services, ICollection<(string, Uri)> mappings)
    {
        var routes = mappings.Select(BuildRouteConfig).ToArray();

        static RouteConfig BuildRouteConfig((string path, Uri _) mapping, int index)
        {
            string pathWithoutTrailingSlash = mapping.path.TrimEnd('/');

            var routeConfig = new RouteConfig()
            {
                RouteId = $"route-{index}",
                ClusterId = $"cluster-{index}",
                Match = new() { Path = $"{pathWithoutTrailingSlash}/{{**path}}" },
            };
            
            return routeConfig.WithTransformPathRemovePrefix(pathWithoutTrailingSlash);
        }

        var clusters = mappings.Select(BuildClusterConfig).ToArray();

        static ClusterConfig BuildClusterConfig((string _, Uri destination) mapping, int index)
            => new()
            {
                ClusterId = $"cluster-{index}",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { $"cluster-{index}-destination-0", new () { Address = mapping.destination.AbsoluteUri } }
                }
            };

        return services
            .AddReverseProxy()
            .LoadFromMemory(routes, clusters);
    }
}
