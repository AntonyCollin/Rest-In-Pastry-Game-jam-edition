using TMPro;
using UnityEngine;

// display ghosts lines
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComp;

    public void NewDialogue(string text, Color color)
    {
        textComp.text = text;
        textComp.color = color;
    }
}
