
using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

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
            await this.AssertImage(containerSettings);
            var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                User = containerSettings.User,
                Name = containerSettings.Name,
                Image = containerSettings.ImageName,
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

        private Task AssertImage(DockerContainerSettings containerSettings)
        {
            string imageName = containerSettings.ImageName;
            if (string.IsNullOrEmpty(containerSettings.ImageName))
            {
                throw new ArgumentException("Image name should not be null or empty.");
            }
            var report = new Progress<JSONMessage>(msg =>
            {
                Debug.WriteLine($"Download {imageName} | {msg.Status}|{msg.ProgressMessage}|{msg.ErrorMessage}");
            });

            if (imageName.IndexOfAny(new char[] { ':', '@' }) < 0)
            {
                imageName = $"{imageName}:latest";
            }
            return dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = imageName,

            }, new AuthConfig(), report);
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
