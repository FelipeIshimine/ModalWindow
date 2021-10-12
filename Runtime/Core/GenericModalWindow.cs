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
        

        public override void RootInitialize(BaseModalMessage baseModalMessage)
        {
            Msg = baseModalMessage as T;
            Initialize();
        }
        protected abstract void Initialize();


        public override void Open(Action callback)
        {
            bgContainer.Open();
            mainContainer.Open(callback);
        }
        
        public override void Close(Action callback)
        {
            void OnCloseDone()
            {
                callback?.Invoke();
                CloseEnd(this);
            }
            
            CloseStart(this);
            bgContainer.Close();
            mainContainer.Close(OnCloseDone);
        }

      
        public override Type GetMessageType() => typeof(T);
        
    }
}