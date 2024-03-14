using System;

namespace GameScript
{
    [Serializable]
    public class DiskLocale
    {
        public uint id;
        public uint index; // Used to lookup localization
        public string name;
        public DiskLocalization localizedName;
    }
}
