using System;
using UnityEngine;

namespace GE.ModalWindows
{
    
    public abstract class BaseModalWindow : MonoBehaviour
    {
        public event Action<BaseModalWindow> OnCloseStart;
        public event Action<BaseModalWindow> OnCloseEnd;

        [SerializeField] protected AnimatedContainer bgContainer;
    
        [SerializeField] protected  AnimatedContainer mainContainer;

        private UnityEngine.Canvas _canvas;

        protected UnityEngine.Canvas Canvas
        {
            get
            {
                if(!_canvas) _canvas = GetComponent<UnityEngine.Canvas>();
                return _canvas;
            }
        }
        
        public abstract void RootInitialize(BaseModalMessage baseModalMessage);
        public void Open() => Open(null);
        public abstract void Open(Action callback);
        
        public void Close() => Close(null);
        public abstract void Close(Action callback);
     
        public abstract Type GetMessageType();

        public void SetSortOrder(int nSortOrder) => Canvas.sortingOrder = nSortOrder;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) 
                SetSortOrder(ModalWindowManager.DefaultLayer);
        }
        #endif

        protected void CloseStart(BaseModalWindow obj) => OnCloseStart?.Invoke(obj);

        protected void CloseEnd(BaseModalWindow obj) => OnCloseEnd?.Invoke(obj);
    }
}