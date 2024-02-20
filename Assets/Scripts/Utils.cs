using System;
using UnityEngine.EventSystems;

public class Utils 
{

    // creating event triggers this way will prevent associating stuff in the editor
    public static EventTrigger.Entry CreateEventTrigger(Action<PointerEventData> function, EventTriggerType type)
    {
        EventTrigger.Entry ev = new EventTrigger.Entry();
        ev.eventID = type;
        ev.callback.AddListener((data) => { function((PointerEventData)data); });
        return ev;
    }
}
