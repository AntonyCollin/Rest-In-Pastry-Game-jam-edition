using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(EventTrigger))]
//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(Image))]
public class Hover : MonoBehaviour
{
    [Tooltip("For cooking choices, hover image will be assigned by script")]
    // deprecated -- didnt know unity had this feature
    [SerializeField] private string audioPath = "Audio/hover-sound";
    [SerializeField] private Sprite hoverImage;
    [SerializeField] private Sprite defaultImg;
    
    private EventTrigger trigger;
    private Image imgComp;
    private AudioSource audioSource;
    private AudioClip hoverSound;

    private void Awake()
    {
        hoverSound = Resources.Load<AudioClip>(audioPath);
        audioSource = GetComponent<AudioSource>();
        imgComp = GetComponent<Image>();
        trigger = gameObject.GetComponent<EventTrigger>();

        // create and associate event trigger
        trigger.triggers.Add(Utils.CreateEventTrigger(PointerEnter, EventTriggerType.PointerEnter));
        trigger.triggers.Add(Utils.CreateEventTrigger(PointerExit, EventTriggerType.PointerExit));
    }

    private void PointerEnter(PointerEventData e)
    {
        audioSource.PlayOneShot(hoverSound);
        imgComp.sprite = hoverImage;
    }

    private void PointerExit(PointerEventData e)
    {
        imgComp.sprite = defaultImg;
    }

    private void OnDisable()
    {
        if (defaultImg != null) { imgComp.sprite = defaultImg; }
    }
}
