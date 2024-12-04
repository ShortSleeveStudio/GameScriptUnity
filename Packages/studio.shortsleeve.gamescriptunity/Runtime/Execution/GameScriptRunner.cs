using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameScript
{
    public class GameScriptRunner : MonoBehaviour
    {
        #region Editor
#if UNITY_EDITOR
        void OnValidate()
        {
            UnityEditor.MonoScript monoScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
            int currentExecutionOrder = UnityEditor.MonoImporter.GetExecutionOrder(monoScript);
            if (currentExecutionOrder != m_ExecutionOrder)
            {
                UnityEditor.MonoImporter.SetExecutionOrder(monoScript, m_ExecutionOrder);
            }
        }
#endif
        #endregion

        #region Singleton
        private static GameScriptRunner Instance { get; set; }
        #endregion

        #region Private State
        // Using linked lists so we can iterate and add
        private LinkedList<RunnerContext> m_ContextsActive;
        private LinkedList<RunnerContext> m_ContextsInactive;
        private Thread m_MainThread;
        private Database m_Database;
        #endregion

        #region Inspector
        [Header("Runner Settings")]
        [SerializeField]
#pragma warning disable CS0414
        private int m_ExecutionOrder = -1;
#pragma warning restore CS0414

        [SerializeField]
        private bool m_DontDestroyOnLoad = true;
        #endregion

        #region API
        public static IEnumerator LoadDatabase()
        {
            yield return Instance.m_Database.Initialize();
        }

        public static ActiveConversation StartConversation(
            ConversationReference conversationRef,
            IRunnerListener listener
        ) => StartConversation(conversationRef.Id, listener);

        public static ActiveConversation StartConversation(
            uint conversationId,
            IRunnerListener listener
        )
        {
            Conversation conversation = Instance.m_Database.FindConversation(conversationId);
            return StartConversation(conversation, listener);
        }

        public static ActiveConversation StartConversation(
            Conversation conversation,
            IRunnerListener listener
        )
        {
            Instance.EnsureMainThread();
            RunnerContext context = Instance.ContextAcquire();
            context.Start(conversation, listener);
            return new(context.SequenceNumber, context.ContextId);
        }

        public static void SetFlag(ActiveConversation active, int flag)
        {
            Instance.EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active);
            if (ctx == null)
                throw new Exception(
                    "You can't set a flag for conversations that have already ended"
                );
            ctx.SetFlag(flag);
        }

        public static void SetFlagForAll(int flag)
        {
            LinkedListNode<RunnerContext> node = Instance.m_ContextsActive.First;
            while (node != null)
            {
                LinkedListNode<RunnerContext> next = node.Next;
                node.Value.SetFlag(flag);
                node = next;
            }
        }

        public static void RegisterFlagListener(ActiveConversation active, Action<int> listener)
        {
            Instance.EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active);
            if (ctx == null)
                throw new Exception(
                    "You can't register a flag listener on a conversation that's already ended"
                );
            ctx.OnFlagRaised += listener;
        }

        public static void UnregisterFlagListener(ActiveConversation active, Action<int> listener)
        {
            Instance.EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active);
            if (ctx == null)
                return;
            ctx.OnFlagRaised -= listener;
        }

        public static bool IsActive(ActiveConversation active)
        {
            Instance.EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active);
            return ctx != null;
        }

        public static void StopConversation(ActiveConversation active)
        {
            Instance.EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active);
            if (ctx == null)
                // we assume the conversation is already ended. Thus this call is idempotent.
                return;
            Instance.ContextRelease(ctx);
        }

        public static void StopAllConversations()
        {
            Instance.EnsureMainThread();
            LinkedListNode<RunnerContext> node = Instance.m_ContextsActive.First;
            while (node != null)
            {
                LinkedListNode<RunnerContext> next = node.Next;
                Instance.ContextRelease(node);
                node = next;
            }
        }

        public static Localization FindLocalization(uint localizationId) =>
            Instance.m_Database.FindLocalization(localizationId);

        public static Locale FindLocale(uint localeId) => Instance.m_Database.FindLocale(localeId);

        public static Conversation FindConversation(uint conversationId) =>
            Instance.m_Database.FindConversation(conversationId);

        public static Property FindProperty(Property[] properties, string propertyName) =>
            Instance.m_Database.FindProperty(properties, propertyName);

        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Singleton
            if (Instance != null)
                Debug.LogWarning("Singleton set multiple times");
            Instance = this;

            // Don't Destroy on Load
            if (m_DontDestroyOnLoad)
                DontDestroyOnLoad(this);

            // Initialize state
            m_Database = new();
            m_ContextsActive = new();
            m_ContextsInactive = new();
            for (uint i = 0; i < Settings.Instance.InitialConversationPool; i++)
            {
                m_ContextsInactive.AddLast(new RunnerContext(Settings.Instance));
            }
            m_MainThread = Thread.CurrentThread;
        }

        private void Update()
        {
            if (m_ContextsActive.Count == 0)
                return;
            LinkedListNode<RunnerContext> iterator = m_ContextsActive.First;
            do
            {
                // Grab value
                RunnerContext runnerContext = iterator.Value;

                // Iterate
                iterator = iterator.Next;

                // Handle current context
                bool isConversationActive = runnerContext.Tick();
                if (!isConversationActive)
                    ContextRelease(runnerContext);
            } while (iterator != null);
        }
        #endregion

        #region Helpers
        private RunnerContext ContextAcquire()
        {
            RunnerContext context;
            if (m_ContextsInactive.Count == 0)
            {
                context = new(Settings.Instance);
                m_ContextsActive.AddLast(context);
            }
            else
            {
                // We add to and remove from the end of the inactive list so that the
                // "primed" contexts are used more frequently. It's more likely that
                // the oft-used contexts will have more room for signals/blocks.
                LinkedListNode<RunnerContext> node = m_ContextsInactive.Last;
                m_ContextsInactive.RemoveLast();
                m_ContextsActive.AddLast(node);
                context = node.Value;
            }
            return context;
        }

        private void ContextRelease(RunnerContext context)
        {
            LinkedListNode<RunnerContext> node = m_ContextsActive.Find(context);
            // Idempotent
            if (node != null)
                ContextRelease(node);
        }

        private void ContextRelease(LinkedListNode<RunnerContext> node)
        {
            node.Value.Stop();
            m_ContextsActive.Remove(node);
            m_ContextsInactive.AddLast(node);
        }

        private RunnerContext FindContextActive(ActiveConversation active)
        {
            LinkedListNode<RunnerContext> node = m_ContextsActive.First;
            while (node != null)
            {
                LinkedListNode<RunnerContext> next = node.Next;
                if (node.Value.ContextId == active.ContextId)
                {
                    if (node.Value.SequenceNumber != active.SequenceNumber)
                        return null;
                    return node.Value;
                }
                node = next;
            }
            return null;
        }

        private void EnsureMainThread()
        {
            if (m_MainThread != Thread.CurrentThread)
                throw new Exception("Runner APIs can only be used from the main thread");
        }
        #endregion
    }
}
