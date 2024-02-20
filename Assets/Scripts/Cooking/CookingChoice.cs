using UnityEngine;

[RequireComponent(typeof(Hover))]
[RequireComponent(typeof(Tooltip))]
[RequireComponent (typeof(AudioSource))]

public class CookingChoice : MonoBehaviour
{
    [HideInInspector]
    public CookingData ingredient;
    public Station station;
    public Player player;
    public PopupManager pm;
    public AudioClip selectSound;

    private AudioSource audioSource;

    public void Choose()
    {
        if (ingredient.isFinalIngredient())
        {
            audioSource.PlayOneShot(station.finalItemSound);
            pm.RemoveCanvasLayer();
            player.AddToInventory(ingredient);
        }
        else
        {
            audioSource.PlayOneShot(selectSound);
            station.DisplayChoices(ingredient.nextChoices);
        }
    }

    private void Awake()
    {
        audioSource = station.GetComponent<AudioSource>();
    }
}
