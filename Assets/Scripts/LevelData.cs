using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]

public class LevelData : ScriptableObject
{
    public DialogueLine[] intro;
    public DialogueLine[] outro;
    [SerializeField] public int maxFailures;
    [SerializeField] public GameObject[] ghosts;
}
