using System;
using System.Collections.Generic;

namespace GameScript
{
    [Serializable]
    public class Disk
    {
        public List<DiskLocalization> localizations;
        public List<DiskLocale> locales;
        public List<DiskActor> actors;
        public List<DiskConversation> conversations;
    }
}
