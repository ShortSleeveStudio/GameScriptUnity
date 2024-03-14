using System;

namespace GameScript
{
    [Serializable]
    public class Locale
    {
        public uint id;
        public uint index; // Used to lookup localization
        public string name;
        public Localization localizedName;
    }
}
