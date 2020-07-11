

namespace Core
{
    public enum GameFileExtension { EXE, PNG, BAT, XML, TXT, JPEG }; //DO I LOOK LIKE I KNOW WHAT A JPEG IS

    public class GameFile
    {
        public string Name { get; set; } = "Unknown";
        public GameFileExtension Extension { get; set; }
        public GameFolder ContainingFolder { get; set; }

        public GameFile(string name, GameFileExtension extension, GameFolder parent)
        {
            Name = name;
            Extension = extension;
            ContainingFolder = parent;
        }
    }
}