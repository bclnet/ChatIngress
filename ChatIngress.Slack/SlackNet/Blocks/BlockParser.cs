using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet;
using SlackNet.Blocks;
using System;
using System.IO;
using System.Text;

namespace ChatIngress.Slack.SlackNet.Blocks
{
    public static class BlockParser
    {
        static readonly SlackJsonSettings DefaultJsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));

        public static Block[] Parse(string source, SlackJsonSettings jsonSettings = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var serializer = JsonSerializer.Create((jsonSettings ?? DefaultJsonSettings).SerializerSettings);
            return JToken.Parse(source)?.ToObject<Block[]>(serializer);
        }

        public static string Parse(Block[] source, SlackJsonSettings jsonSettings = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var serializer = JsonSerializer.Create((jsonSettings ?? DefaultJsonSettings).SerializerSettings);
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                serializer.Serialize(w, source);
                w.Flush();
                return Encoding.UTF8.GetString(s.ToArray());
            }
        }

        public static ModalViewDefinition ParseModalView(string source, SlackJsonSettings jsonSettings = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var serializer = JsonSerializer.Create((jsonSettings ?? DefaultJsonSettings).SerializerSettings);
            return JToken.Parse(source)?.ToObject<ModalViewDefinition>(serializer);
        }

        public static string ParseModalView(ModalViewDefinition source, SlackJsonSettings jsonSettings = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            DefaultJsonSettings.SerializerSettings.Formatting = Formatting.Indented;
            var serializer = JsonSerializer.Create((jsonSettings ?? DefaultJsonSettings).SerializerSettings);
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                serializer.Serialize(w, source);
                w.Flush();
                return Encoding.UTF8.GetString(s.ToArray());
            }
        }
    }
}
