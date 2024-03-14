using System;

namespace GameScript
{
    [Serializable]
    public class Localization
    {
        public uint id;
        public string[] localizations; // Lookup with locale.index
    }
}
