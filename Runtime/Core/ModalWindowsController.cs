using System;
using System.Collections.Generic;
using UnityEngine;

namespace GE.ModalWindows
{
    public class ModalWindowsController : BaseMonoSingleton<ModalWindowsController>
    {
        public static event Action OnBegin;
        public static event Action OnDone;

        private readonly Dictionary<string, Queue<BaseModalWindow>> _availableWindows = new Dictionary<string, Queue<BaseModalWindow>>();

        private readonly Queue<BaseModalMessage> _queue = new Queue<BaseModalMessage>();

        private readonly Stack<BaseModalWindow> _activeModals = new Stack<BaseModalWindow>();

        public static int ActiveModalsCount => Instance._activeModals.Count;
        public static int QueueCount => Instance._queue.Count;
        public static bool IsShowing { get; private set; } = false;

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
            string messageTypeName = baseModalWindow.GetMessageType().FullName;
            Debug.Log($"Enqueue Windows of type: {messageTypeName}");
            _availableWindows[messageTypeName].Enqueue(baseModalWindow);
        }
        private static void OpenNextMessage()
        {
            if (!IsShowing)
            {
                IsShowing = true;
                OnBegin?.Invoke();
            }
            BaseModalMessage baseModalMessage = Instance._queue.Dequeue();
            OpenMessage(baseModalMessage);
        }

        private static void OpenMessage(BaseModalMessage baseModalMessage)
        {
            Debug.Log(baseModalMessage.GetType().FullName);
            BaseModalWindow baseModalWindow = Instance.DequeueWindow(baseModalMessage.GetType().FullName);
            baseModalWindow.RootInitialize(baseModalMessage);
            Instance._activeModals.Push(baseModalWindow);
            baseModalWindow.SetSortOrder(ModalWindowManager.DefaultLayer + Instance._activeModals.Count);
            baseModalWindow.Open();
        }

        public static void CloseCurrent()
        {
            //Sacamos de la lista de ventanas activas
            BaseModalWindow baseModalWindow = Instance._activeModals.Pop(); 
            //lo hacemos cerrarse y despues encolarse en las ventanas disponibles
            baseModalWindow.mainContainer.Close(()=> Instance.EnqueueWindow(baseModalWindow));

            if (Instance._activeModals.Count == 0)
            {
                if (Instance._queue.Count > 0)
                    OpenNextMessage();
                else
                    Done();
            }
        }

        private static void Done()
        {
            IsShowing = false;
            OnDone?.Invoke();
            OnDone = null;
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