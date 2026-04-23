using System;
using UnityEngine;
using UnityEngine.UI;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードの外見を設定するためのクラス
    /// </summary>
    public class CardDesign : MonoBehaviour
    {
        CardBase parent;
        [NonSerialized] public RectTransform rectTransform;
        [NonSerialized] public Vector2 initialPosition;
        Image image;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="cardBase">カードの親</param>
        public void Initialize(CardBase cardBase)
        {
            parent = cardBase;
            rectTransform = gameObject.GetComponent<RectTransform>();
            image = gameObject.GetComponent<Image>();
            image.sprite = parent.SkillData.Icon;
        }
    }
}