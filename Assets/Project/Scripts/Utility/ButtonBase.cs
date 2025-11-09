using UnityEngine.EventSystems;
public interface ButtonBase : IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    void Hover() { }

    void DeHover() { }

    void Push() { }

    void DePush() { }
}