﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuantumTek.QuantumDialogue.Editor
{
    [System.Serializable]
    public class QD_ConversationNode : QD_Node
    {
        //public new string WindowTitle => Data.Name;
        public new string WindowTitle => "Conversation"; // phill changed this 

        public QD_Conversation Data;

        public QD_ConversationNode(int id, QD_NodeType type, float x = 0, float y = 0) : base(id, type, x, y)
        {
            Type = QD_NodeType.Conversation;
            Window = new Rect(0, 0, 200, 65);
            Inputs = new List<QD_Knob>
            { };
            Outputs = new List<QD_Knob>
            {
                new QD_Knob(0, "First Message", QD_KnobType.Output, 40, false)
            };
            AllowedInputs = new List<QD_ConnectionRule>
            { };
            AllowedOutputs = new List<QD_ConnectionRule>
            {
                new QD_ConnectionRule(QD_NodeType.Message, 1, QD_NodeType.Conversation, 0)
            };
        }

        public override void DrawWindow(int id)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(new Rect(5, 35, 50, 20), "Name", QD_DialogueEditor.skin.label);
            string fName = EditorGUI.TextField(new Rect(60, 35, 135, 20), Data.Name, QD_DialogueEditor.skin.textField);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(QD_DialogueEditor.db.DataDB);
                Data.Name = fName;
                QD_DialogueEditor.db.DataDB.SetConversation(Data.ID, Data);
            }

            GUI.DragWindow();
        }

        /// <summary>
        /// The function called to modify the node's data whenever a node connects.
        /// </summary>
        /// <param name="dialogue">The dialogue data.</param>
        /// <param name="connectionType">The type of the connecting node.</param>
        /// <param name="connectionID">The id of the connecting node.</param>
        /// <param name="connectionKnobID">The id of the connecting knob.</param>
        /// <param name="knobID">The id of this node's knob.</param>
        /// <param name="knobType">The type of this node's knob.</param>
        public override void OnConnect(QD_Dialogue dialogue, QD_NodeType connectionType, int connectionID, int connectionKnobID, int knobID, QD_KnobType knobType)
        {
            Data.OnConnect(dialogue, connectionType, connectionID, connectionKnobID, knobID, knobType);
            QD_DialogueEditor.db.DataDB.SetConversation(Data.ID, Data);
        }

        /// <summary>
        /// The function called to modify the node's data whenever a node disconnects.
        /// </summary>
        /// <param name="dialogue">The dialogue data.</param>
        /// <param name="connectionType">The type of the connecting node.</param>
        /// <param name="connectionID">The id of the connected node.</param>
        public override void OnDisconnect(QD_Dialogue dialogue, QD_NodeType connectionType, int connectionID)
        {
            Data.OnDisconnect(dialogue, connectionType, connectionID);
            QD_DialogueEditor.db.DataDB.SetConversation(Data.ID, Data);
        }

        /// <summary>
        /// The function called to modify the node's data whenever a node disconnects.
        /// </summary>
        /// <param name="dialogue">The dialogue data.</param>
        /// <param name="connectionType">The type of the connecting node.</param>
        /// <param name="connectionID">The id of the connected node.</param>
        /// <param name="knobID">The id of the knob.</param>
        /// <param name="knobType">The type of the knob.</param>
        public override void OnDisconnect(QD_Dialogue dialogue, QD_NodeType connectionType, int connectionID, int knobID, QD_KnobType knobType)
        {
            Data.OnDisconnect(dialogue, connectionType, connectionID, knobID, knobType);
            QD_DialogueEditor.db.DataDB.SetConversation(Data.ID, Data);
        }

        /// <summary>
        /// The function called to modify the node's data whenever a node disconnects.
        /// </summary>
        /// <param name="dialogue">The dialogue data.</param>
        /// <param name="connectionType">The type of the connecting node.</param>
        /// <param name="connectionID">The id of the connected node.</param>
        /// <param name="connectionKnobID">The id of the connected knob.</param>
        /// <param name="knobID">The id of the knob.</param>
        /// <param name="knobType">The type of the knob.</param>
        public override void OnDisconnect(QD_Dialogue dialogue, QD_NodeType connectionType, int connectionID, int connectionKnobID, int knobID, QD_KnobType knobType)
        {
            Data.OnDisconnect(dialogue, connectionType, connectionID, connectionKnobID, knobID, knobType);
            QD_DialogueEditor.db.DataDB.SetConversation(Data.ID, Data);
        }
    }
}