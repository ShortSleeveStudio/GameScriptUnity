using System;

namespace GameScript
{
    [Serializable]
    public class Locale
    {
        public uint Id;
        public uint Index; // Used to lookup localization
        public string Name;
        public Localization LocalizedName;
    }
}
