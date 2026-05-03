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

        /// <summary>
        /// アクティブ状態の処理
        /// </summary>
        public abstract void SetActive();
        /// <summary>
        /// 非アクティブ状態の処理
        /// </summary>
        public abstract void SetInActive();
        /// <summary>
        /// ホバー状態の処理
        /// </summary>
        protected abstract void Hover();
        /// <summary>
        /// 非ホバー状態の処理
        /// </summary>
        protected abstract void UnHover();
        /// <summary>
        /// ボタン押し込み時の処理
        /// </summary>
        protected abstract void Push();
        /// <summary>
        /// ボタンを離した時の処理
        /// </summary>
        protected abstract void Release();
    }
}