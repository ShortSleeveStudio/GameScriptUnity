using System;

namespace GameScript
{
    [Serializable]
    public class Localization
    {
        public uint Id;
        public string[] Localizations; // Lookup with Locale.Index
    }
}
