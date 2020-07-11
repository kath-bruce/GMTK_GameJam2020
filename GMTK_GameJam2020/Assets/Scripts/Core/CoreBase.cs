namespace Core
{
    public abstract class CoreBase
    {
        public string Name { get; set; } = "Unknown";
        public GameFolder ContainingFolder { get; set; }

        public CoreBase(string name, GameFolder parent)
        {
            Name = name;
            ContainingFolder = parent;
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