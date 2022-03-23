using System;
using UnityEngine;

namespace GE.ModalWindows
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public abstract class GenericModalWindow<T> : BaseModalWindow where T : BaseModalMessage
    {
        protected T Msg { get; private set; }

        private bool _isOpen = false;

        
        public override void RootInitialize(BaseModalMessage baseModalMessage)
        {
            Msg = baseModalMessage as T;
            Initialize();
        }
        protected abstract void Initialize();

        public override void Open(Action callback)
        {
            if (_isOpen) throw new Exception($"{this} Modal is Already Open");
            _isOpen = true;
            bgContainer.Open();
            Action openCallback = OnOpenDone;
            openCallback += callback;
            mainContainer.Open(openCallback);
        }
        
        public override void Close(Action callback)
        {
            if (!_isOpen) throw new Exception($"{this} Modal is Already Closed");
            
            _isOpen = false;
            
            Action closeCallback = OnCloseDone;
            closeCallback += callback;
            
            CloseStart(this);
            bgContainer.Close();
            mainContainer.Close(closeCallback);
        }

      
        public override Type GetMessageType() => typeof(T);

        protected virtual void OnOpenDone() { }
        
        protected virtual void OnCloseDone()
        {
            CloseEnd(this);
        }
    }
}