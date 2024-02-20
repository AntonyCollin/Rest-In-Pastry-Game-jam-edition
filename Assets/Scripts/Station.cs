using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent (typeof(AudioSource))]

// make it interactble interface ?
public class Station : MonoBehaviour
{
    public Canvas canvas;
    public GameObject keyCanvas;
    public CookingData[] firstChoices;
    public AudioClip finalItemSound;
    public AudioClip interactionSound;

    private Transform panel;
    private Animator anim;
    
    void Awake()
    {
        TryGetComponent(out Animator an);
        anim = an;
        panel = canvas.transform.GetChild(0);
    }

    public void PlayAnim()
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.Play("final",0);
        }
    }

    public void Display()
    {
        DisplayChoices(firstChoices);
    }

    public void DisplayChoices(CookingData[] cookingChoices)
    {
        ResetDisplay();

        for (int i = 0; i < cookingChoices.Length; i++)
        {
            // get by public variables instead of child
            panel.transform.GetChild(i).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = cookingChoices[i].image;
            panel.transform.GetChild(i).GetComponent<Tooltip>().ingredient = cookingChoices[i];
            panel.transform.GetChild(i).gameObject.SetActive(true);
            panel.transform.GetChild(i).gameObject.GetComponent<CookingChoice>().ingredient = cookingChoices[i];
        }
    }

    private void ResetDisplay()
    {
        foreach (Transform child in panel.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
