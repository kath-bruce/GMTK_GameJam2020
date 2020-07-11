using System.Collections.Generic;

namespace Core
{
    public class GameFolder
    {
        public string Name { get; set; } = "Unknown";
        public List<GameFolder> Folders { get; set; } = new List<GameFolder>();
        public List<GameFile> Files { get; set; } = new List<GameFile>();
        public GameFolder ContainingFolder { get; set; }

        public GameFolder(string name, GameFolder parent)
        {
            Name = name;
            ContainingFolder = parent;
        }

    }
}