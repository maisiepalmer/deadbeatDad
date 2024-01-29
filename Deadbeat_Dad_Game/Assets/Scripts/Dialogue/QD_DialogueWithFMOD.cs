using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEditor;

namespace QuantumTek.QuantumDialogue.Demo
{
    /// <summary>
    /// QD_DialogueWithFMOD 
    /// 
    /// intented to be added as a component to an NPC character
    /// attach a collider (with isTrigger true) to begin the conversation
    /// 
    /// use SetConversation() to switch conversation name without
    /// beginning the dialogue screen/sound. This is useful when you wish to 
    /// prepare an NPC to start a new conversation with the player after
    /// completing an objective elsewhere in the level for example.
    /// 
    /// </summary>
    public class QD_DialogueWithFMOD : MonoBehaviour
    {
        public QD_DialogueHandler handler; // must be set to the Dialogue Handler in hierarchy

        public QD_Dialogue dialogue = null; // runtime QD_Dialogue nodes for this character, must be set

        public string conversationName = "Meeting Bob";

        private bool conversationEnded; // conversations without loops can 'end'

        public FMODUnity.EventReference eventPath;

        public PlayerController player;

        // dialogue FMOD event should trigger "programmer instrument" callback
        private FMOD.Studio.EVENT_CALLBACK callbackDelegate;

        // call this to remotely set runtime dialogue database for character
        public void SetDialogueRuntime(QD_Dialogue newDialogue)
        {
            dialogue = newDialogue;
        }

        // call this to remotely dialogue entrypoint for next collider trigger
        public void SetConversation(string newName)
        {
            conversationName = newName;
        }

        //////// //////// //////// //////// //////// //////// //////// ////////
        /// MonoBehaviour functions
        /// 

        void Start()
        {
            // Explicitly create a delegate object and assign it to a member so 
            // the C# garbage collector doesn't clean up while it's being used
            callbackDelegate = new FMOD.Studio.EVENT_CALLBACK(FMODDialogueCallback);
        }

        private void OnTriggerEnter(Collider collider)
        {
            BeginConversation();
        }

        private void OnTriggerExit(Collider other)
        {
            Reset();
        }

        private void OnEnable()
        {
            Reset();
        }

        //////// //////// //////// //////// //////// //////// //////// ////////
        /// functions that control dialogue flow
        /// 

        private void BeginConversation()
        {
            // abort if dialogue for this character unset
            if (dialogue == null)
            {
                Debug.LogError("dialogue not set for " + this.gameObject.name);
                return;
            }

            // abort if conversation not found
            if (dialogue.GetConversation(conversationName) == null)
            {
                Debug.LogError(conversationName + " conversation not found");
                return;
            }

            // tell dialogue handler the current character is speaking
            handler.currentController = this;
            conversationEnded = false;
            handler.SetConversation(conversationName);
            handler.ShowDialogueCanvas(true);

            player.LockMovement(true);

            SetText();
        }

        //////// //////// //////// //////// //////// //////// //////// ////////
        /// MonoBehaviour functions
        /// 

        private void FMODStartDialogueEvent(string key)
        {
            Debug.Log("FMOD " + eventPath + " (key " + key + ")");

            // create an instance
            var instance = FMODUnity.RuntimeManager.CreateInstance(eventPath);

            // Pin the key string in memory so the C# garbage collector 
            // doesn't clean up whilst it's being used
            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);

            // pass a pointer to the pinned key string in memory
            instance.setUserData(GCHandle.ToIntPtr(stringHandle));

            // we control the programmer instrument with this callback
            instance.setCallback(callbackDelegate);

            // for 3d sound events, we must attach to this game object
            Transform tf = gameObject.GetComponent<Transform>(); // for pos, orientation
            Rigidbody rb = gameObject.GetComponent<Rigidbody>(); // for velo, accel
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, tf, rb);

            // make a jazz noise here
            instance.start();
            instance.release(); // immediately release
        }

        // android/ios platforms MUST be informed this is an AOT MONO P INVOKE CALLBACK
        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT FMODDialogueCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr stringPtr;
            instance.getUserData(out stringPtr);

            // Get the string object
            GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
            String key = stringHandle.Target as String;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains(".")) // key includes file extention e.g. ".wav" ".ogg"
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else // perform audio table lookup with key
                    {
                        FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
                
                // TODO - potentially interact with 'handler.canAdvance
                //        to en/disable "click to continue" behaviour
                
                //case FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_PLAYED:
                //{
                //    Debug.Log("FMOD sound played");
                //    break;
                //}
                //case FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                //{
                //    Debug.Log("FMOD sound stopped");
                //    break;
                //}
            }
            return FMOD.RESULT.OK;
        }

        //////// //////// //////// //////// //////// //////// //////// ////////
        /// functions that manipulate QD_DialogueHandler
        /// 

        // set dialogue canvas and mouse visibility (called by OnTriggerExit)
        private void Reset()
        {
            conversationEnded = false;
            handler.ShowDialogueCanvas(false);
            handler.SetDefaultMouseLockAndHide();
        }

        // called (from QD_ChoiceButton) when a player makes a dialogue choice 
        public void Choose(int choice)
        {
            if (conversationEnded)
                return;

            Next(choice);
        }

        // advance to the next dialogue entry
        public void Next(int choice = -1)
        {
            if (conversationEnded)
                return;

            // Go to the next message
            handler.NextMessage(choice);
            // Set the new text
            SetText();
            // End if there is no next message
            if (handler.currentMessageInfo.ID < 0)
            {
                conversationEnded = true;
                player.LockMovement(false);

                // Hide canvas when conversation is over
                handler.ShowDialogueCanvas(false);
            }
        }

        // setup dialogue canvas and compute programmer instrument key
        // to control which line of dialogue we should play
        private void SetText()
        {
            // Clear everything
            handler.speakerName.text = "";
            handler.messageText.gameObject.SetActive(false);
            handler.messageText.text = "";
            handler.ClearChoices();

            // If at the end, don't do anything
            if (conversationEnded)
                return;

            // Generate and show choices, otherwise display message
            if (handler.currentMessageInfo.Type == QD_NodeType.Choice)
            {
                // override mouse cursor state
                handler.SetMouseLockAndHide(false);
                handler.speakerName.text = "Player";
                handler.GenerateChoices();
            }
            else if (handler.currentMessageInfo.Type == QD_NodeType.Message)
            {
                // restore default mouse cursor state
                handler.SetDefaultMouseLockAndHide();

                QD_Message message = handler.GetMessage();
                handler.speakerName.text = message.SpeakerName;
                handler.messageText.text = message.MessageText;

                handler.messageText.gameObject.SetActive(true);

                // now compute the programmer instrument key
                // to control which line of dialogue we should play

                // keys use the speaker name without spaces all lowercase
                string speakerNameLowercaseNoSpaces =
                    message.SpeakerName.Replace(@" ", "").ToLower();

                // keys use the phrase number
                string phraseNumberAsString = message.AudioPhrase.ToString();

                // keys use the first word of the message text all lowercase
                // any punctuation will be omitted, other characters are fine
                // e.g. message text "huh?!, who said that?" becomes "huh"
                // e.g. message text ":) Привет!" becomes "привет"

                string[] words = message.MessageText.Split(' ');
                string firstWordLowercase = "";
                foreach(string word in words)
                {
                    string disalowedCharacters = @"[\p{P}]"; // punctuation character class

                    string withoutPunctuation = Regex.Replace(word, disalowedCharacters, "");
                    if ( withoutPunctuation.Length > 0 )
                    {
                        firstWordLowercase = withoutPunctuation.ToLower();
                        break;
                    }
                }

                // key examples "bob-0-greetings", "ronburgundy-31-baxter"
                string key = speakerNameLowercaseNoSpaces + "-"
                    + phraseNumberAsString + "-"
                    + firstWordLowercase;

                FMODStartDialogueEvent(key);
            }
        }




    }
}