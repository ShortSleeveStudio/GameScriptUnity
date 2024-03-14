using System;

namespace GameScript
{
    [Serializable]
    public class GameData
    {
        public Localization[] localizations;
        public Locale[] locales;
        public Actor[] actors;
        public Conversation[] conversations;
    }
}
