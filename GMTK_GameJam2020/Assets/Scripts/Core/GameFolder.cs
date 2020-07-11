using System.Collections.Generic;

namespace Core
{
    public class GameFolder : CoreBase
    {
        public List<GameFolder> Folders { get; set; } = new List<GameFolder>();
        public List<GameFile> Files { get; set; } = new List<GameFile>();

        public GameFolder(string name, GameFolder parent) : base(name, parent) { }

        public GameFile GetOlderSibling(GameFile file)
        {
            int currIndex = Files.FindIndex(0, (gf) => gf.Equals(file));
            return Files[--currIndex];
        }

        public GameFolder GetOlderSibling(GameFolder folder)
        {
            int currIndex = Folders.FindIndex(0, (gf) => gf.Equals(folder));
            return Folders[--currIndex];
        }

        public GameFile GetYoungerSibling(GameFile file)
        {
            int currIndex = Files.FindIndex(0, (gf) => gf.Equals(file));
            return Files[++currIndex];
        }

        public GameFolder GetYoungerSibling(GameFolder folder)
        {
            int currIndex = Folders.FindIndex(0, (gf) => gf.Equals(folder));
            return Folders[++currIndex];
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Folders.Count.GetHashCode() ^ Files.Count.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            if (!(obj is GameFolder))
            {
                return false;
            }

            var folder = obj as GameFolder;

            if (folder.Folders.Count == Folders.Count && folder.Files.Count == Files.Count)
            {
                return true;
            }

            return false;
        }

    }
}