using Core;
using System.IO;
using Newtonsoft.Json;

namespace Helpers
{
    public static class JsonHelper
    {
        public static void SaveJson(GameFolder root, string path)
        {
            var writer = new StreamWriter(path);

            var serialiser = JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });//.Serialize(writer, root);

            serialiser.Serialize(new JsonTextWriter(writer), root);

            writer.Flush();

            writer.Close();

            //File.WriteAllText(path, json.ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public static GameFolder LoadJson(string path)
        {
            var reader = new JsonTextReader(new StreamReader(path));

            var root = JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            }).Deserialize<GameFolder>(reader);

            reader.Close();

            return root;
        }
    }
}