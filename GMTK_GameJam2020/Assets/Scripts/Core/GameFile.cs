

namespace Core
{
    public enum GameFileExtension { EXE, PNG, BAT, XML, TXT, JPEG }; //DO I LOOK LIKE I KNOW WHAT A JPEG IS

    public class GameFile : CoreBase
    {
        public GameFileExtension Extension { get; set; }

        public GameFile(string name, GameFileExtension extension, GameFolder parent) : base(name, parent)
        {
            Extension = extension;
        }
    }
}