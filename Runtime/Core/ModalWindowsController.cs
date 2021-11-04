using System;
using System.Collections.Generic;
using UnityEngine;

namespace GE.ModalWindows
{
    public class ModalWindowsController : BaseMonoSingleton<ModalWindowsController>
    {
        public static bool IsBusy => QueueCount > 0 || ActiveModalsCount > 0;
        private static event Action OnBegin;
        private static event Action OnDone;

        private readonly Dictionary<string, Queue<BaseModalWindow>> _availableWindows = new Dictionary<string, Queue<BaseModalWindow>>();

        private readonly Queue<BaseModalMessage> _queue = new Queue<BaseModalMessage>();

        private readonly Stack<BaseModalWindow> _activeModals = new Stack<BaseModalWindow>();

        public static BaseModalWindow CurrentActiveModal=>Instance._activeModals.Peek();
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
                Debug.Log("ModalWindowsController: BEGIN");
                IsShowing = true;
                OnBegin?.Invoke();
            }
            BaseModalMessage baseModalMessage = Instance._queue.Dequeue();
            OpenMessage(baseModalMessage);
        }

        private static void OpenMessage(BaseModalMessage baseModalMessage)
        {
            
            BaseModalWindow baseModalWindow = Instance.DequeueWindow(baseModalMessage.GetType().FullName);

            baseModalWindow.OnCloseStart += ModalWindowCloseStart;
            baseModalWindow.OnCloseEnd += ModalWindowCloseEnd;
            
            baseModalWindow.RootInitialize(baseModalMessage);
            Instance._activeModals.Push(baseModalWindow);
            Debug.Log($"ModalWindowController PUSH:{baseModalWindow}");

            baseModalWindow.SetSortOrder(ModalWindowManager.DefaultLayer + Instance._activeModals.Count);
            baseModalWindow.Open(null);
        }
   
        private static void Done()
        {
            Debug.Log("ModalWindowsController: DONE");
            IsShowing = false;
            OnDone?.Invoke();
            OnDone = null;
        }

        private static void ModalWindowCloseStart(BaseModalWindow baseModalWindow)
        {
            baseModalWindow.OnCloseStart -= ModalWindowCloseStart;

            Debug.Log($"CloseStart:{baseModalWindow.gameObject.name}");
            if (CurrentActiveModal == baseModalWindow)
            {
                //Sacamos de la lista de ventanas activas
                
                var modal = Instance._activeModals.Pop();
                Debug.Log($"ModalWindowController POP:{modal}");

                if (Instance._activeModals.Count == 0)
                {
                    if (Instance._queue.Count > 0)
                        OpenNextMessage();
                    else
                        Done();
                }
            }
            else
                throw new Exception($"Error: Esta intentando cerrar una ventana que no es la actual. \n Actual:{CurrentActiveModal.gameObject.name} Cerrando:{baseModalWindow.gameObject.name}");
        }
        
        private static void ModalWindowCloseEnd(BaseModalWindow baseModalWindow)
        {
            baseModalWindow.OnCloseEnd -= ModalWindowCloseEnd;
            
            //Encolarse en las ventanas disponibles
            Instance.EnqueueWindow(baseModalWindow);
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

        /// <summary>
        /// El evento se borra luego de ejecutarse
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterOnDone(Action callback) => OnDone += callback;
        /// <summary>
        /// El evento se borra luego de ejecutarse
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterOnBegin(Action callback) => OnBegin += callback;
    }
}