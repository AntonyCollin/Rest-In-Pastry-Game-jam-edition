using UnityEngine;

[CreateAssetMenu(fileName = "GhostDialogue", menuName = "ScriptableObjects/GhostDialogue")]

public class GhostDialogue : ScriptableObject
{
    public string line;
    public bool isMissingOrder = false;
    public Color color = Color.white;
}
