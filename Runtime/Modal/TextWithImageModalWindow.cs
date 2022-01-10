using System;
using TMPro;
using UnityEngine.UI;

namespace GE.ModalWindows.Modal
{
    public class TextWithImageModalWindow : GenericModalWindow<TextWithImageMessage>
    {
        public TextMeshProUGUI content;
        public Image img;
        protected override void Initialize()
        {
            img.sprite = Msg.sprite;
            content.text = Msg.body;
        }

        public override void Close(Action callback)
        {
            Msg.Callback?.Invoke();
            base.Close(callback);
        }
    }
}