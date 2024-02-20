using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "ScriptableObjects/Ghost")]
[RequireComponent(typeof(GhostComponent))]
public class GhostData : ScriptableObject
{
    public enum Type { Green, Blue, Red, Boss }
    public Type ghostType;
    public int timeBeforeLeaving;
    public int nbOfOrders = 1;
    public GhostDialogue[] dialogues;
    public GhostDialogue[] missingOrderDialogues;

}
