using TMPro;
using UI.ModalWindows.Core;
using UI.ModalWindows.Messages;

namespace UI.ModalWindows.Modal
{
    public class TextOnlyModalWindow : GenericModalWindow<TextOnlyModalMessage>
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;
        private TextOnlyModalMessage _message;
        protected override void Initialize()
        {
            _message = Message;
            if (string.IsNullOrEmpty(Message.title))
            {
                title.gameObject.SetActive(false);
            }
            else
            {
                title.gameObject.SetActive(true);
                title.text = Message.title;
            }
            content.text = Message.body;
        }

        public override void Close()
        {
            _message.Callback?.Invoke();
            base.Close();
        }

  
    }
}
