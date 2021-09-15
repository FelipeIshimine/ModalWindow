using System;
using UnityEngine;

namespace GE.ModalWindows
{
    [System.Serializable]
    public class TextWithImageMessage : BaseModalMessage
    {
        public Sprite sprite;
        public string body;
        public bool pauseTime;
        public Action Callback; 

        public TextWithImageMessage(Sprite sprite, string body, bool pauseTime, Action callback = null)
        {
            this.sprite = sprite;
            this.body = body;
            this.pauseTime = pauseTime;
            Callback = callback;
        }
    }
}