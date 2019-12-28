
using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YS.Docker.Impl.Dotnet
{
    [ServiceClass]
    public class DotnetDockerContainerService : IDockerContainerService
    {
        public DotnetDockerContainerService(IDockerClient dockerClient)
        {
            this.dockerClient = dockerClient;
        }
        private IDockerClient dockerClient;
        public async Task<string> RunAsync(DockerContainerSettings containerSettings)
        {
            if (containerSettings == null)
            {
                throw new ArgumentNullException(nameof(containerSettings));
            }
            var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                User = containerSettings.User,
                Name = containerSettings.Name,
                Image = containerSettings.ImageName,
                AttachStdin = false,
                AttachStdout = true,
                AttachStderr = true,
                Env = CreateEnvs(containerSettings),
                ExposedPorts = CreateExposedPorts(containerSettings),
                Cmd = (containerSettings.Commands ?? new string[] { }).ToList(),

                HostConfig = new HostConfig
                {
                    PortBindings = this.CreatePortBinding(containerSettings)
                }
            });

            var startOk = await dockerClient.Containers.StartContainerAsync(response.ID,
               new ContainerStartParameters { });
            if (!startOk)
            {
                throw new Exception($"Failed to start container {response.ID}.");
            }
            return response.ID;
        }
        private IList<string> CreateEnvs(DockerContainerSettings containerSettings)
        {
            if (containerSettings.Envs == null) return new List<string>();
            return containerSettings.Envs.Select(kv => $"{kv.Key}={kv.Value}").ToList();
        }
        private IDictionary<string, EmptyStruct> CreateExposedPorts(DockerContainerSettings containerSettings)
        {
            if (containerSettings.Ports == null) return new Dictionary<string, EmptyStruct>();
            return containerSettings.Ports.ToDictionary(kv => kv.Key.ToString(), kv => default(EmptyStruct));
        }

        private IDictionary<string, IList<PortBinding>> CreatePortBinding(DockerContainerSettings containerSettings)
        {
            if (containerSettings.Ports == null) return new Dictionary<string, IList<PortBinding>>();
            return containerSettings.Ports.ToDictionary(
                kv => kv.Key.ToString(),
                kv => new List<PortBinding> { new PortBinding { HostPort = kv.Value.ToString() } } as IList<PortBinding>);
        }

        public async Task StopAsync(string containerId)
        {
            if (containerId == null)
            {
                throw new ArgumentNullException(nameof(containerId));
            }

            await dockerClient.Containers.StopContainerAsync(containerId,
                 new ContainerStopParameters { });
            await dockerClient.Containers.RemoveContainerAsync(containerId,
                new ContainerRemoveParameters { });
        }


    }
}
