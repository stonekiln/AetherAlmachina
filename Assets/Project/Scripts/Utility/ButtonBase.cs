using UnityEngine.EventSystems;
public interface IButtonBase : IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    void Hover() { }

    void DeHover() { }

    void Push() { }

    void DePush() { }
}