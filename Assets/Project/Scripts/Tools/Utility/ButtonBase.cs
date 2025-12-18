using System;
using R3.Triggers;

namespace Utility
{
    /// <summary>
    /// UIのボタンを実装する際に継承するクラス
    /// </summary>
    public abstract class ButtonBase : ObservableEventTrigger
    {
        /// <summary>
        /// クリックしたとき実行する関数
        /// </summary>
        public Action onClickCallback;
        /// <summary>
        /// マウスがボタンの上に配置されているか
        /// </summary>
        [NonSerialized] public bool isHover;
        /// <summary>
        /// マウスがボタンを選択しているか
        /// </summary>
        [NonSerialized] public bool isSelect;

        public abstract void SetActive();
        public abstract void SetInActive();
        protected abstract void Hover();
        protected abstract void UnHover();
        protected abstract void Push();
        protected abstract void Release();
    }
}