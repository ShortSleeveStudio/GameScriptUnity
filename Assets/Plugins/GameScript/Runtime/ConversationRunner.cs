using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class ConversationRunner : MonoBehaviour
    {
        #region State
        private Settings m_Settings;
        private LinkedList<ConversationContext> m_ContextsActive;
        private LinkedList<ConversationContext> m_ContextsInactive;
        #endregion

        #region API
        // public Conversation StartConversation(int conversationId)
        // {

        // }
        #endregion

        #region Unity Lifecycle Methods
        void Awake()
        {
            // Initialize state
            m_Settings = Resources.Load<Settings>(RuntimeConstants.k_SettingsAssetName);
            m_ContextsActive = new();
            m_ContextsInactive = new();
            EnsurePoolSize((int)m_Settings.InitialConversationPool);

            // TODO - TEMPORARY, DELETE THIS
            // tmp = new ConversationContext(m_Settings);
            // test = RoutineDirectory.Directory[2];
        }
        // private ConversationContext tmp;
        // private Action<ConversationContext> test;

        void Update()
        {
            // if (tmp != null)
            // {
            //     test(tmp);
            //     if (tmp.IsRoutineExecuted()) tmp = null;
            // }
        }
        #endregion

        #region Helpers
        private void EnsurePoolSize(int poolSize)
        {
            for (int i = m_ContextsInactive.Count; i < poolSize; i++)
            {
                m_ContextsInactive.AddLast(new ConversationContext(m_Settings));
            }
        }
        #endregion
    }
}
