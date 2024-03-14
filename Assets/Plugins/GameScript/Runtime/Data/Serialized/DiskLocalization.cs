using System;

namespace GameScript
{
    [Serializable]
    public class DiskLocalization
    {
        public uint id;
        public string[] localizations; // Lookup with locale.index
    }
}
