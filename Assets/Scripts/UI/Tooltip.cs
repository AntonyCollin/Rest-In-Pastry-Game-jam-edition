using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Tooltip : MonoBehaviour
{
    [HideInInspector]
    public CookingData ingredient;
    [HideInInspector]
    public bool isFromBlueGhost = false;
    public InspectionManager ins;

    private EventTrigger trigger;

    private void Awake()
    {
        trigger = GetComponent<EventTrigger>();

        trigger.triggers.Add(Utils.CreateEventTrigger(PointerEnter, EventTriggerType.PointerEnter));
        trigger.triggers.Add(Utils.CreateEventTrigger(PointerExit, EventTriggerType.PointerExit));
    }

    public void PointerEnter(PointerEventData e)
    {
        ins.NewInspection(ingredient, isFromBlueGhost);
    }

    public void PointerExit(PointerEventData e)
    {
        ins.ClearInspection();
    }

    private void OnDisable()
    {
        ins.ClearInspection();
    }
}
