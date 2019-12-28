using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace YS.Docker.Impl.Dotnet
{
    [OptionsClass]
    public class DockerOptions
    {
        public DockerOptions()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                this.EndPoint = "npipe://./pipe/docker_engine";
            }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
            {
                this.EndPoint = "unix:/var/run/docker.sock";
            }
        }
        public string EndPoint { get; set; }

    }
}
