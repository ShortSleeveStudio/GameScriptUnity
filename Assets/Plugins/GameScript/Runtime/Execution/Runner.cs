using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameScript
{
    public class Runner : MonoBehaviour
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
        private static Runner Instance { get; set; }
        #endregion

        #region Private State
        // Using linked lists so we can iterate and add
        private LinkedList<RunnerContext> m_ContextsActive;
        private LinkedList<RunnerContext> m_ContextsInactive;
        private Thread m_MainThread;
        #endregion

        #region Inspector Variables
        [Header("Runner Settings")]
        [SerializeField]
#pragma warning disable CS0414
        private int m_ExecutionOrder = -1;
#pragma warning restore CS0414

        [SerializeField]
        private bool m_Singleton = true;

        [SerializeField]
        private bool m_DontDestroyOnLoad = true;
        #endregion

        #region API
        public static ActiveConversation StartConversation(
            ConversationReference conversationRef,
            IRunnerListener listener
        ) => StartConversation(conversationRef.Id, listener);

        public static ActiveConversation StartConversation(
            uint conversationId,
            IRunnerListener listener
        )
        {
            Conversation conversation = Database.FindConversation(conversationId);
            return StartConversation(conversation, listener);
        }

        public static ActiveConversation StartConversation(
            Conversation conversation,
            IRunnerListener listener
        )
        {
            EnsureMainThread();
            RunnerContext context = Instance.ContextAcquire();
            context.Start(conversation, listener);
            return new(context.SequenceNumber, context.ContextId);
        }

        public static void StopConversation(ActiveConversation active)
        {
            EnsureMainThread();
            RunnerContext ctx = Instance.FindContextActive(active.CancellationToken);
            if (ctx.SequenceNumber == active.SequenceNumber)
                Instance.ContextRelease(ctx);
            // else - we assume the conversation is already ended. Thus this call is idempotent.
        }

        public static void StopAllConversations()
        {
            EnsureMainThread();
            foreach (RunnerContext context in Instance.m_ContextsActive)
            {
                Instance.ContextRelease(context);
            }
        }

        private static void EnsureMainThread()
        {
            if (Instance.m_MainThread != Thread.CurrentThread)
                throw new Exception("Runner APIs can only be used from the main thread");
        }
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            // Singleton
            if (m_Singleton)
            {
                if (Instance != null)
                    Debug.LogWarning("Singleton set multiple times");
                Instance = this;
            }

            // Don't Destroy on Load
            if (m_DontDestroyOnLoad)
                DontDestroyOnLoad(this);

            // Initialize state
            m_ContextsActive = new();
            m_ContextsInactive = new();
            for (uint i = 0; i < Settings.Instance.InitialConversationPool; i++)
            {
                m_ContextsInactive.AddLast(new RunnerContext(Settings.Instance));
            }
            m_MainThread = Thread.CurrentThread;

            // Load conversation database
            GameData data = Database.Instance;
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
            context.Stop();
            LinkedListNode<RunnerContext> node = m_ContextsActive.Find(context);
            m_ContextsActive.Remove(node);
            m_ContextsInactive.AddLast(node);
        }

        private RunnerContext FindContextActive(uint contextId)
        {
            foreach (RunnerContext context in m_ContextsActive)
            {
                if (context.ContextId == contextId)
                    return context;
            }
            return null;
        }
        #endregion
    }
}
