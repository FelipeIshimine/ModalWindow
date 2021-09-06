using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ModalWindows.Core;
using UI.ModalWindows.Messages;
using TMPro;

namespace UI.ModalWindows.Modal
{
    public class TextWithImageWindow : GenericModalWindow<TextWithImageMessage>
    {
        public Image image;
        public TextMeshProUGUI TextMesh;
        private TextWithImageMessage _message;

        protected override void Initialize()
        {
            if (Message.pauseTime)
                Time.timeScale = 0;

            _message = Message;
            image.sprite = Message.sprite;
            TextMesh.text = Message.body;
        }

        public override void Close()
        {
            if (_message.pauseTime)
                Time.timeScale = 1;

            _message.Callback?.Invoke();
            base.Close();
        }
    }
}
