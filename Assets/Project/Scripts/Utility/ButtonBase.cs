using System;
using R3.Triggers;

namespace Utility
{
    public abstract class ButtonBase : ObservableEventTrigger
    {
        public Action onClickCallback;
        public bool isHover;
        public bool isSelect;

        public abstract void SetActive();
        public abstract void SetInActive();
        protected abstract void Hover();
        protected abstract void UnHover();
        protected abstract void Push();
        protected abstract void Release();
    }
}