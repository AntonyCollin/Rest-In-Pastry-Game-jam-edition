using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/DialogueLine")]
[RequireComponent(typeof(GhostComponent))]
public class DialogueLine : ScriptableObject
{
    public enum Position { Left, Right, Center, Outside, LowerCenter};

    public Sprite background;
    public Sprite[] sprites;
    public Position[] spritePositions;
    public string speakerName = "Margo";
    
    [SerializeField] public string line;
    public bool isLastLine = false;
    public bool isFirstLine = false;
}
