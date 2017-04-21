using System;

namespace Quic.Config
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Module : Attribute
    {
        public Module(string dir, string name, string version)
        {
            Dir = dir;
            Name = name;
            Version = version;
            name = "";
        }

        public string Dir { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}