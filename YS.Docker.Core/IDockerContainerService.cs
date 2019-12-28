
using System.Threading.Tasks;

namespace YS.Docker
{
    public interface IDockerContainerService
    {
        Task<string> RunAsync(DockerContainerSettings containerSettings);
        Task StopAsync(string containerId);
    }
}
