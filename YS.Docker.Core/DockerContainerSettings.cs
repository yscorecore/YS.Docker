using System;
using System.Collections.Generic;
using System.IO;

namespace YS.Docker
{
    public class DockerContainerSettings
    {
        public string User { get; set; }
        public string ImageName { get; set; }

        public string Name { get; set; }

        public IDictionary<string, string> Envs { get; set; }

        public IDictionary<int, int> Ports { get; set; }

        public IDictionary<string, string> Volumns { get; set; }
        public bool Background { get; set; } = false;
        public string[] Commands { get; set; }

    }
}
