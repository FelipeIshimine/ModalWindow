using System;
using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public abstract class GenericModalWindow<T> : BaseModalWindow where T : BaseModalMessage
    {
        protected T Message { get; private set; }

        public override void RootInitialize(BaseModalMessage baseModalMessage)
        {
            Message = baseModalMessage as T;
            Initialize();
        }
        protected abstract void Initialize();
        
        public override void Open()
        {
            mainContainer.Open();
        }
        
        public override void Close()
        {
            ModalWindowsController.Close(this);
        }

      
        public override Type GetMessageType() => typeof(T);
        
        
        private void OnValidate()
        {
            if (!mainContainer)
            {
                mainContainer = GetComponentInChildren<AnimatedContainer>();
                if (!mainContainer)
                {
                    mainContainer = new GameObject("MainContainer",typeof(RectTransform), typeof(AnimatedContainer)).GetComponent<AnimatedContainer>();
                    mainContainer.transform.SetParent(transform);
                }
            }
        }
    }
