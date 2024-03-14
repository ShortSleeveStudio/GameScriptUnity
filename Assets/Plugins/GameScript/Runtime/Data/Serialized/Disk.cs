using System;

namespace GameScript
{
    [Serializable]
    public class Disk
    {
        public DiskLocalization[] localizations;
        public DiskLocale[] locales;
        public DiskActor[] actors;
        public DiskConversation[] conversations;
    }
}
