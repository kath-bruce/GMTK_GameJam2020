using System;

namespace Core
{
    public enum InfectionState { Null = -1, Clean, Infected, Quarantined }
    public enum ScanResult { Null = -1, License, PasswordLetter, Joke }

    public abstract class CoreBase
    {
        public string Name { get; set; } = "Unknown";
        public GameFolder ContainingFolder { get; set; }
        public InfectionState InfectionState { get; set; }
        public ScanResult Result { get; set; }

        public CoreBase(string name, GameFolder parent, InfectionState infectionState, ScanResult result)
        {
            Name = name;
            ContainingFolder = parent;
            InfectionState = infectionState;
            Result = result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CoreBase))
            {
                return false;
            }

            var core = obj as CoreBase;

            if (Name == core.Name && ContainingFolder == core.ContainingFolder)
            {
                return true;
            }

            return false;
        }
    }
}