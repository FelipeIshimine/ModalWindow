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
            _message = ModalMessage;
            if (string.IsNullOrEmpty(ModalMessage.title))
            {
                title.gameObject.SetActive(false);
            }
            else
            {
                title.gameObject.SetActive(true);
                title.text = ModalMessage.title;
            }
            content.text = ModalMessage.body;
        }

        public override void Close()
        {
            _message.Callback?.Invoke();
            base.Close();
        }

  
    }
}
