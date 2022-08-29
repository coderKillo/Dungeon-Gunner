using UnityEngine;

public enum MouseEvent
{
    None,
    RightClickDown,
    RightClickUp,
    RightClickDrag,
    LeftClickDown,
    LeftClickUp,
    LeftClickDrag,
}

static public class MouseEventHelper
{
    public static MouseEvent GetEvent(Event mouseEvent)
    {
        switch (mouseEvent.type)
        {
            case EventType.MouseDown:
                if (mouseEvent.button == 1)
                {
                    return MouseEvent.RightClickDown;
                }
                else if (mouseEvent.button == 0)
                {
                    return MouseEvent.LeftClickDown;
                }
                break;
            case EventType.MouseDrag:
                if (mouseEvent.button == 1)
                {
                    return MouseEvent.RightClickDrag;
                }
                else if (mouseEvent.button == 0)
                {
                    return MouseEvent.LeftClickDrag;
                }
                break;
            case EventType.MouseUp:
                if (mouseEvent.button == 1)
                {
                    return MouseEvent.RightClickUp;
                }
                else if (mouseEvent.button == 0)
                {
                    return MouseEvent.LeftClickUp;
                }
                break;
            default:
                break;
        }

        return MouseEvent.None;
    }
}