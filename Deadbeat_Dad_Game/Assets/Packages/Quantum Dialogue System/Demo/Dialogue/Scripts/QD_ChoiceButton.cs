using UnityEngine;

namespace QuantumTek.QuantumDialogue.Demo
{
    public class QD_ChoiceButton : MonoBehaviour
    {
        public int number;

        // phill edited this
        //public QD_DialogueDemo conversation;
        public QD_DialogueHandler handler = null;

        //public void Select() => converation.Choose(number);
        public void Select() => handler.Choose(number);

    }
}