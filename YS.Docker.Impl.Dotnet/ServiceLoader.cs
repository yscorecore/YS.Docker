using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Docker.Impl.Dotnet
{
    public class ServiceLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetConfigOrNew<DockerOptions>();
            services.AddSingleton<IDockerClient, DockerClient>((sc)=> 
            {
                var dockerConfig = new DockerClientConfiguration(new Uri(options.EndPoint));
                return dockerConfig.CreateClient();
            });
        }
    }
}
