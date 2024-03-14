using System;

namespace GameScript
{
    [Serializable]
    public class DiskLocalization
    {
        public uint id;
        // Lookup using the index field added to locales
        public string[] localizations;
    }
}
