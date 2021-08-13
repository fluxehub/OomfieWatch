using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace StormWatch
{
    public class Gifs
    {
        public string Online { get; set; }
        public string Offline { get; set; }
    }
    
    public class Config
    {
        public string Token { get; set; }
        public ulong Channel { get; set; }
        public ulong User { get; set; }
        public Gifs Gifs { get; set; }

        public static Config FromYaml(string path)
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<Config>(yaml);
        }
    }
}