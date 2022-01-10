using System;
using TMPro;
using GE.ModalWindows;

namespace GE.ModalWindows.Modal
{
    public class TextOnlyModalWindow : GenericModalWindow<TextOnlyModalMessage>
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;
        private TextOnlyModalMessage _message;
        protected override void Initialize()
        {
            _message = Msg;
            if (string.IsNullOrEmpty(Msg.title))
            {
                title.gameObject.SetActive(false);
            }
            else
            {
                title.gameObject.SetActive(true);
                title.text = Msg.title;
            }
            content.text = Msg.body;
        }

        public override void Close(Action callback)
        {
            _message.Callback?.Invoke();
            base.Close(callback);
        }
    }
}
