namespace GameScript
{
    public class Game
    {
        private GameLocalization[] m_Localizations;
        private GameLocale[] m_Locales;
        private GameActor[] m_Actors;
        private GameConversation[] m_Conversations;

        public Game(
            GameLocalization[] localizations, GameLocale[] locales,
            GameActor[] actors, GameConversation[] conversations)
        {
            m_Localizations = localizations;
            m_Locales = locales;
            m_Actors = actors;
            m_Conversations = conversations;
        }
    }
}

