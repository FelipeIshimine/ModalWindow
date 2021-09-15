using System;
using UnityEngine;

namespace GE.ModalWindows
{
    [System.Serializable]
    public class TextOnlyModalMessage : BaseModalMessage
    {
        public string body;
        public string title;
        public Action Callback;
        
        public TextOnlyModalMessage(string body, Action callback = null)
        {
            this.body = body;
            Callback = callback;
        }
        
        public TextOnlyModalMessage(string title, string body, Action callback = null)
        {
            this.title = title;
            this.body = body;
            Callback = callback;
        }
    }
}    

