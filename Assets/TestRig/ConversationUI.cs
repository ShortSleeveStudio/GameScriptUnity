using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameScript;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConversationUI : MonoBehaviour, IRunnerListener
{
    #region Constants
    private const int k_ReadTimeMillis = 1000;
    #endregion

    #region Inspector Variables
    [SerializeField]
    private GameObject m_HistoryContent;

    [SerializeField]
    private GameObject m_HistoryItemPrefab;

    [SerializeField]
    private GameObject m_ChoiceContent;

    [SerializeField]
    private GameObject m_ChoiceItemPrefab;

    [SerializeField]
    private ScrollRect m_HistoryScrollRect;

    [SerializeField]
    private TesterSettings m_TestSettings;
    #endregion

    #region State
    private Action<ConversationUI> m_OnComplete;
    #endregion

    #region Initialization
    public void Initialize(Action<ConversationUI> onComplete) => m_OnComplete = onComplete;
    #endregion

    #region Runner Listerner
    public void OnConversationEnter(Conversation conversation, OnReady onReady)
    {
        onReady();
    }

    public void OnConversationExit(Conversation conversation, OnReady onReady)
    {
        for (int i = m_HistoryContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(m_HistoryContent.transform.GetChild(i).gameObject);
        }
        onReady();
        m_OnComplete(this);
    }

    public void OnNodeDecision(List<Node> nodes, OnDecisionMade onDecisionMade)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            GameObject choiceGO = Instantiate(m_ChoiceItemPrefab);
            ChoiceUI choiceUI = choiceGO.GetComponent<ChoiceUI>();
            string buttonText = "";
            if (node.UIResponseText != null)
                buttonText = node.UIResponseText.GetLocalization(m_TestSettings.CurrentLocale);
            choiceUI.SetButtonText(buttonText);
            choiceUI.RegisterButtonHandler(() =>
            {
                onDecisionMade(node);
            });
            choiceGO.transform.SetParent(m_ChoiceContent.transform);
        }
    }

    public void OnNodeEnter(Node node, OnReady onReady)
    {
        if (node.VoiceText != null)
        {
            GameObject historyItemGO = Instantiate(m_HistoryItemPrefab);
            HistoryItemUI historyItem = historyItemGO.GetComponent<HistoryItemUI>();
            string actorName = node.Actor.LocalizedName.GetLocalization(
                m_TestSettings.CurrentLocale
            );
            string voiceText = node.VoiceText.GetLocalization(m_TestSettings.CurrentLocale);
            historyItem.SetVoiceText(voiceText);
            historyItem.SetActorName(actorName);
            historyItemGO.transform.SetParent(m_HistoryContent.transform);
            // m_HistoryScrollRect.normalizedPosition = Vector2.zero;
            Delay(k_ReadTimeMillis, onReady);
        }
        else
            onReady();
    }

    public void OnNodeExit(Node node, OnReady onReady)
    {
        for (int i = m_ChoiceContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(m_ChoiceContent.transform.GetChild(i).gameObject);
        }
        onReady();
    }

    public void OnError(Conversation conversation, Exception e) => Debug.LogException(e);
    #endregion

    #region Helpers
    private async void Delay(int millis, OnReady onReady)
    {
        await Task.Delay(millis);
        onReady();
    }
    #endregion
}
