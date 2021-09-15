using System;
using UnityEngine;

namespace GE.ModalWindows
{
    public abstract class BaseModalWindow : MonoBehaviour
    {
        public AnimatedContainer mainContainer;

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
    
        public abstract void Close();
        public abstract void Open();
        public abstract Type GetMessageType();

        public void SetSortOrder(int nSortOrder) => Canvas.sortingOrder = nSortOrder;

        private void OnValidate()
        {
            if(!Application.isPlaying) SetSortOrder(ModalWindowManager.DefaultLayer);
        }
    }
}