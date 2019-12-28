using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Docker.Impl.Dotnet.UnitTest
{
    [TestClass]
    public class DotnetDockerContainerServiceTest
    {
        private IDockerContainerService dockerContainerService;
        [TestInitialize]
        public void Setup()
        {
            this.OnSetup();
        }
        protected virtual void OnSetup()
        {
            this.dockerContainerService = this.OnCreateDockerContainerService();
        }

        private IDockerContainerService OnCreateDockerContainerService()
        {
            var host = Knife.Hosting.Host.CreateHost();
            return host.Services.GetRequiredService<IDockerContainerService>();
        }


        [TestMethod]
        public async Task TestEchoHello()
        {
            string cid = await dockerContainerService.RunAsync(new DockerContainerSettings
            {
                ImageName = "centos:latest",
                Commands = new[] { "echo", "hello" },
            });
            Assert.IsNotNull(cid);
            await dockerContainerService.StopAsync(cid);

        }
    }
}
