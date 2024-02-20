using TMPro;
using UnityEngine;

/// <summary>
/// change inner text of button depending on if save exists
/// </summary>
public class NewGame : MonoBehaviour
{
    public GameObject newGameText;
    public GameObject loadGameBtn;
    public Save save;

    private void Awake()
    {
       save = Save.GetSave();

        newGameText.GetComponent<TextMeshProUGUI>().text = save.level == 1 ? "NEW GAME" : "CONTINUE";
        loadGameBtn.SetActive(save.level != 1);
    }
}
