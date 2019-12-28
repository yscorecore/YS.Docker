using Knife.Hosting.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace YS.Docker.Impl.Dotnet.UnitTest
{
    [TestClass]
    public class DotnetDockerContainerServiceTest:TestBase<IDockerContainerService>
    {
        [TestMethod]
        public async Task TestEchoHello()
        {
            string cid = await TestObject.RunAsync(new DockerContainerSettings
            {
                ImageName = "centos",
                Commands = new[] { "echo", "hello" },
            });
            Assert.IsNotNull(cid);
            await TestObject.StopAsync(cid);
        }
    }
}
