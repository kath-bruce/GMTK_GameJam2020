using System;

namespace Core
{
    public enum GameFileExtension { EXE, PNG, BAT, XML, TXT, JPEG }; //DO I LOOK LIKE I KNOW WHAT A JPEG IS

    public class GameFile : CoreBase
    {
        public GameFileExtension Extension { get; set; }
        public bool IsVirus { get; set; }

        public GameFile(string name, GameFileExtension extension, GameFolder parent, InfectionState infectionState, ScanResult result, bool isVirus)
        : base(name, parent, infectionState, result)
        {
            IsVirus = isVirus;
            Extension = extension;
        }
    }
}