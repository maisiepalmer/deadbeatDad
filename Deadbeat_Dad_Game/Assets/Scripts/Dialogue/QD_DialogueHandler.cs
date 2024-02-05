using UnityEngine;
using TMPro;
using System.Collections.Generic;
using QuantumTek.QuantumDialogue.Demo;

namespace QuantumTek.QuantumDialogue
{
    /// <summary>
    /// Gives basic information about a message, including its id, type (either message or choice), and next message id.
    /// </summary>
    [System.Serializable]
    public struct QD_MessageInfo
    {
        public int ID;
        public int NextID;
        public QD_NodeType Type;

        public QD_MessageInfo(int id, int nextID, QD_NodeType type)
        {
            ID = id;
            NextID = nextID;
            Type = type;
        }
    }

    /// <summary>
    /// QD_DialogueHandler is responsible for controlling a dialogue and the conversations within.
    /// </summary>
    [AddComponentMenu("Quantum Tek/Quantum Dialogue/Dialogue Handler")]
    [DisallowMultipleComponent]
    public class QD_DialogueHandler : MonoBehaviour
    {
        [HideInInspector] public int currentConversationIndex = -1;
        [HideInInspector] public QD_Conversation currentConversation;
        [HideInInspector] public QD_MessageInfo currentMessageInfo;

        public PlayerController player;
        public StateHandler state;

        // phill moved this 
        /// 
        /// For 3D first person games the mouse often hidden and locked whilst
        /// the mouse controls the camera orientation. This must be overriden
        /// when a user chooses a dialogue message from the dialogue panel.
        /// 
        /// For 2D poin-and-click games obviously this isn't the case.
        /// set this as appropriate
        public bool defaultMouseLock = true;

        /// <summary>
        /// Apply default mouse lock and hide state
        /// </summary>
        public void SetDefaultMouseLockAndHide()
        {
            SetMouseLockAndHide(defaultMouseLock);
        }

        /// <summary>
        /// Locks and hides mouse cursor (also used to unlock and show)
        /// </summary>
        public void SetMouseLockAndHide(bool newState)
        {
            CursorLockMode newMode = (newState)
                ? CursorLockMode.Locked : CursorLockMode.None;

            Cursor.visible = !newState;
            Cursor.lockState = newMode;
        }

        // phill added this - reference to controller
        [HideInInspector]
        public QD_DialogueWithFMOD currentController = null;

        // phill added this - reference to controller choice
        public void Choose(int choice)
        {
            if (currentController == null) return; // abort if unset

            currentController.Choose(choice);
            state.HandlePlayerReactions(choice, currentMessageInfo.ID);
        }

        // TODO - potentially draw from FMOD callback (SOUND_PLAYED, _STOPPED)
        //        to en/disable "click to continue" behaviour
        //[HideInInspector]
        //public bool canAdvance = true; 

        // phill moved this
        [Header("TODO - hide from inspector - autoset in code")]
        public Canvas dialogueCanvas;

        // phill moved this and made public
        public void ShowDialogueCanvas(bool isEnabled)
        {
            // abort if canvas unset
            if (dialogueCanvas == null) return; 

            dialogueCanvas.enabled = isEnabled;
        }

        // phill moved these
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI messageText;

        // phill moved these
        private List<TextMeshProUGUI> activeChoices = new List<TextMeshProUGUI>();
        private List<TextMeshProUGUI> inactiveChoices = new List<TextMeshProUGUI>();

        // phill moved these
        public Transform choices;
        public TextMeshProUGUI choiceTemplate;

        // phill moved this and made public
        public void ClearChoices()
        {
            for (int i = activeChoices.Count - 1; i >= 0; --i)
            {
                // Use object pooling with the choices to prevent unecessary garbage collection
                activeChoices[i].gameObject.SetActive(false);
                activeChoices[i].text = "";
                inactiveChoices.Add(activeChoices[i]);
                activeChoices.RemoveAt(i);
            }
        }

        // phill moved this and made public
        public void GenerateChoices()
        {
            // Exit if not a choice
            if (currentMessageInfo.Type != QD_NodeType.Choice)
                return;
            // Clear the old choices
            ClearChoices();
            // Generate new choices
            QD_Choice choice = GetChoice();
            int added = 0;
            // Use inactive choices instead of making new ones, if possible
            while (inactiveChoices.Count > 0 && added < choice.Choices.Count)
            {
                int i = inactiveChoices.Count - 1;
                TextMeshProUGUI cText = inactiveChoices[i];
                cText.text = choice.Choices[added];
                QD_ChoiceButton button = cText.GetComponent<QD_ChoiceButton>();
                button.number = added;
                cText.gameObject.SetActive(true);
                activeChoices.Add(cText);
                inactiveChoices.RemoveAt(i);
                added++;
            }
            // Make new choices if any left to make
            while (added < choice.Choices.Count)
            {
                TextMeshProUGUI newChoice = Instantiate(choiceTemplate, choices);
                newChoice.text = choice.Choices[added];
                QD_ChoiceButton button = newChoice.GetComponent<QD_ChoiceButton>();
                button.number = added;
                newChoice.gameObject.SetActive(true);
                activeChoices.Add(newChoice);
                added++;
            }
        }

        // // // // // // // // // // // // // // // // // // // // // 
        //// monobehaviour methods
        ///

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            state = FindObjectOfType<StateHandler>();
        }

        // phill added this
        private void OnEnable()
        {
            // hide dialogue canvas and configure default mouse behaviour
            ShowDialogueCanvas(false);
            SetDefaultMouseLockAndHide();
        }

        // phill moved this
        private void Update()
        {
            // abort if no controller is set
            if (currentController == null) return; 

            // abort if current dialogue is a player choice
            if (currentMessageInfo.Type == QD_NodeType.Choice) return;

            // advance on mouse click primary (0) button
            if (Input.GetMouseButtonDown(0))
            {
                currentController.Next();
            }
        }

        // // // // // // // // // // // // // // // // // // // // // 

        /// <summary>
        /// Returns the current message, if it is a message node.
        /// </summary>
        /// <returns></returns>
        public QD_Message GetMessage()
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return null; 
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return null;

            if (currentMessageInfo.Type != QD_NodeType.Message)
                return null;
            return dialogue.GetMessage(currentMessageInfo.ID);
        }

        /// <summary>
        /// Returns the current choice, if it is a choice node.
        /// </summary>
        /// <returns></returns>
        public QD_Choice GetChoice()
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return null; 
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return null;

            if (currentMessageInfo.Type != QD_NodeType.Choice)
                return null;
            return dialogue.GetChoice(currentMessageInfo.ID);
        }

        /// <summary>
        /// Sets the current conversation based on the name of it.
        /// </summary>
        /// <param name="name">The name of the conversation.</param>
        public void SetConversation(string name)
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return;
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return;

            currentConversationIndex = dialogue.GetConversationIndex(name);
            if (currentConversationIndex < 0 || currentConversationIndex >= dialogue.Conversations.Count)
                return;
            currentConversation = dialogue.Conversations[currentConversationIndex];
            currentMessageInfo = new QD_MessageInfo(currentConversation.FirstMessage, GetNextID(currentConversation.FirstMessage), QD_NodeType.Message);

            player.LockMovement(true);
        }

        /// <summary>
        /// Returns the id of the next message.
        /// </summary>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="choice">The choice, if the current message is a choice. By default -1, meaning the current node is a message node.</param>
        /// <returns></returns>
        public int GetNextID(int id, int choice = -1)
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return -1;
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return -1;

            int nextID = -1;
            QD_NodeType type = GetMessageType(id);
            if (type == QD_NodeType.Message)
            {
                QD_Message m = dialogue.GetMessage(id);
                if (m.NextMessage >= 0)
                    nextID = m.NextMessage;
            }
            else if (type == QD_NodeType.Conversation)
            {
                QD_Choice c = dialogue.GetChoice(id);
                if (c.NextMessages[choice] >= 0)
                    nextID = c.NextMessages[choice];
            }
            return nextID;
        }

        /// <summary>
        /// Returns the type of a message with the given ID. Base if there is no next message.
        /// </summary>
        /// <param name="id">The id of the message or choice.</param>
        /// <returns></returns>
        public QD_NodeType GetMessageType(int id)
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return QD_NodeType.Base;
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return QD_NodeType.Base; 

            if (dialogue.GetMessageIndex(id) >= 0)
                return QD_NodeType.Message;
            else if (dialogue.GetChoiceIndex(id) >= 0)
                return QD_NodeType.Choice;
            return QD_NodeType.Base;
        }

        /// <summary>
        /// Goes to the next message and returns its ID and type. Base if there is no next message, but otherwise Message or Choice.
        /// </summary>
        /// <param name="choice">The choice, if the current message is a choice. By default -1, meaning the current node is a message node.</param>
        /// <returns></returns>
        public QD_MessageInfo NextMessage(int choice = -1)
        {
            // phill added this - abort if QD_Dialogue unset
            if (currentController == null) return new QD_MessageInfo(-1, -1, QD_NodeType.Base);
            QD_Dialogue dialogue = currentController.dialogue;
            if (dialogue == null) return new QD_MessageInfo(-1, -1, QD_NodeType.Base);

            if (currentMessageInfo.NextID < 0 && choice == -1)
            {
                currentMessageInfo = new QD_MessageInfo(-1, -1, QD_NodeType.Base);
                player.LockMovement(false);
                return currentMessageInfo;
            }

            QD_NodeType type = GetMessageType(currentMessageInfo.NextID);
            int id = -1;
            int nextID = -1;
            
            if (currentMessageInfo.Type == QD_NodeType.Message)
            {
                id = currentMessageInfo.NextID;
                
                if (type == QD_NodeType.Message && id >= 0)
                {
                    QD_Message m = dialogue.GetMessage(id);
                    nextID = m.NextMessage;
                }
            }
            else if (currentMessageInfo.Type == QD_NodeType.Choice && choice >= 0)
            {
                QD_Choice c = dialogue.GetChoice(currentMessageInfo.ID);
                currentMessageInfo.NextID = c.NextMessages[choice];
                id = currentMessageInfo.NextID;
                type = QD_NodeType.Message;
                QD_Message m = dialogue.GetMessage(id);
                nextID = m.NextMessage;
            }

            currentMessageInfo = new QD_MessageInfo(id, nextID, type);
            return currentMessageInfo;
        }

        //---------------------------------------------------------------------------------
        // MY OWN CODE:
        public void HandleReactions(string speaker, string phraseNumberAsString, string firstWordLowercase)
        {
            if (speaker == "worker" && phraseNumberAsString == "11") // success
            {
                state.HasFood();
                // make a sound/ set a parameter?!
            }
            else if (speaker == "worker" && phraseNumberAsString == "4") // failure
            {
                // make a sound/ set a parameter?!
            }
            else if (speaker == "sally" && phraseNumberAsString == "7") // success
            {
                state.HasPresent();
                // make a sound/ set a parameter?!
            }
            else if (speaker == "sally" && phraseNumberAsString == "5") // failure
            {
                // make a sound/ set a parameter?!
            }
        }
    }
}