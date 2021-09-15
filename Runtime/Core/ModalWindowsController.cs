using System;
using System.Collections.Generic;
using UnityEngine;

namespace GE.ModalWindows
{
    public class ModalWindowsController : MonoSingleton<ModalWindowsController>
    {
        private readonly Dictionary<string, Queue<BaseModalWindow>> _availableWindows = new Dictionary<string, Queue<BaseModalWindow>>();

        private readonly Queue<BaseModalMessage> _queue = new Queue<BaseModalMessage>();

        private readonly Stack<BaseModalWindow> _activeModals = new Stack<BaseModalWindow>();

        private const float ZOffset = 0.1f;

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            GameObject go = new GameObject("ModalWindowsController",typeof(ModalWindowsController));
            DontDestroyOnLoad(go);
        }
        
        private BaseModalWindow DequeueWindow(string messageTypeName)
        {
            Debug.Log($"DequeueWindow({messageTypeName})");
            if (_availableWindows.TryGetValue(messageTypeName, out Queue<BaseModalWindow> queue))
            {
                var window = queue.Dequeue();
                window.gameObject.SetActive(true);
                return window;
            }
            _availableWindows[messageTypeName] = new Queue<BaseModalWindow>();
            var nWindow = Instantiate(ModalWindowManager.GetWindow(messageTypeName)).GetComponent<BaseModalWindow>();
            DontDestroyOnLoad(nWindow.gameObject);
            return nWindow;
        }

        private void EnqueueWindow(BaseModalWindow baseModalWindow)
        {
            baseModalWindow.gameObject.SetActive(false);
            string messageTypeName = baseModalWindow.GetMessageType().Name;
            Debug.Log($"Enqueue Windows of type: {messageTypeName}");
            _availableWindows[messageTypeName].Enqueue(baseModalWindow);
        }
        private static void OpenNextMessage()
        {
            BaseModalMessage baseModalMessage = Instance._queue.Dequeue();
            OpenMessage(baseModalMessage);
        }

        private static void OpenMessage(BaseModalMessage baseModalMessage)
        {
            Debug.Log(baseModalMessage.GetType().Name);
            BaseModalWindow baseModalWindow = Instance.DequeueWindow(baseModalMessage.GetType().Name);
            baseModalWindow.RootInitialize(baseModalMessage);
            Instance._activeModals.Push(baseModalWindow);
            baseModalWindow.SetSortOrder(ModalWindowManager.DefaultLayer + Instance._activeModals.Count);
            baseModalWindow.Open();
        }

        public static void CloseCurrent()
        {
            BaseModalWindow baseModalWindow = Instance._activeModals.Pop(); //Sacamos de la lista de ventanas activas
            baseModalWindow.mainContainer.Close(()=> Instance.EnqueueWindow(baseModalWindow)); //lo hacemos cerrarse y despues encolarse en las ventanas disponibles
            if(Instance._queue.Count > 0 && Instance._activeModals.Count == 0)
                OpenNextMessage();
        }

        public static void Close(BaseModalWindow baseModalWindow) 
        {
            Debug.Log($"Close:{baseModalWindow.gameObject.name}");
            if(Instance._activeModals.Peek() == baseModalWindow)
                CloseCurrent();
            else
                throw new Exception("Error: Esta intentando cerrar una ventana que no es la actual");
        }

        public static void Enqueue(BaseModalMessage baseModalMessage)
        {
            Instance._queue.Enqueue(baseModalMessage);
            if(Instance._activeModals.Count == 0)
                OpenNextMessage();
        }

        public static void Show(BaseModalMessage baseModalMessage)
        {
            OpenMessage(baseModalMessage);
        }
    }
}