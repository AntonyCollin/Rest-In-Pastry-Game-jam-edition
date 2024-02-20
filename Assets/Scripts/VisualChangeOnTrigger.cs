using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class VisualChangeOnTrigger : MonoBehaviour
{
    public Sprite outlineSprite;
    public Color newColor = Color.white;
    public GameObject objectToChange;

    private Sprite defaultSprite;
    private SpriteRenderer spriteComp;
    private Image imageComp;
    private Color defaultColor;

    private void Awake()
    {
        if (objectToChange == null) 
        {
            objectToChange = gameObject;        
        }

        if (objectToChange.TryGetComponent(out SpriteRenderer sr))
        {
            spriteComp = sr;
            defaultSprite = spriteComp.sprite;
            defaultColor = spriteComp.color;
        }

        if (objectToChange.TryGetComponent(out Image im))
        {
            imageComp = im;
            defaultSprite = imageComp.sprite;
            defaultColor = im.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (spriteComp != null)
        {
            spriteComp.sprite = outlineSprite == null ? defaultSprite : outlineSprite;
            spriteComp.color = newColor;
        }

        if (imageComp != null)
        {
            imageComp.sprite = outlineSprite == null ? defaultSprite : outlineSprite;
            imageComp.color = newColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (spriteComp != null)
        {
            spriteComp.sprite = defaultSprite;
            spriteComp.color = defaultColor;
        }

        if (imageComp != null)
        {
            imageComp.sprite = defaultSprite;
            imageComp.color = defaultColor;
        }
    }
}
