using System.Collections.Generic;
using System;

namespace Core
{
    public class GameFolder : CoreBase
    {
        public List<GameFolder> Folders { get; set; } = new List<GameFolder>();
        public List<GameFile> Files { get; set; } = new List<GameFile>();

        public GameFolder(string name, GameFolder parent, InfectionState infectionState) : base(name, parent, infectionState) { }

        public GameFile GetVirus()
        {
            foreach (var file in Files)
            {
                if (file.IsVirus)
                {
                    return file;
                }
            }

            foreach (var folder in Folders)
            {
                return RecursiveGetVirus(folder);
            }

            return null;
        }

        private GameFile RecursiveGetVirus(GameFolder folder)
        {
            foreach (var file in folder.Files)
            {
                if (file.IsVirus)
                {
                    return file;
                }
            }

            foreach (var innerFolder in folder.Folders)
            {
                return RecursiveGetVirus(innerFolder);
            }

            return null;
        }

        public void SetParents()
        {
            foreach (var file in Files)
            {
                file.ContainingFolder = this;
            }

            foreach (var folder in Folders)
            {
                folder.ContainingFolder = this;

                RecursiveSetParents(folder);
            }
        }

        private void RecursiveSetParents(GameFolder folder)
        {
            foreach (var file in folder.Files)
            {
                file.ContainingFolder = folder;
            }

            foreach (var innerFolder in folder.Folders)
            {
                innerFolder.ContainingFolder = folder;

                RecursiveSetParents(innerFolder);
            }
        }

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