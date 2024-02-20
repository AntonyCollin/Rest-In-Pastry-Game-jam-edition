
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChoice : MonoBehaviour
{
    public Transform levelsParent;
    public Save save;
    public LevelData[] levels;
    public bool isPlayingOutro = false;

    private void Awake()
    {
        // so that when continuing game it plays current saved level
        save = Save.GetSave();
        Save.SetSave(save.level, 0);
    }

    public void DisplayChoices()
    {
        levelsParent.parent.gameObject.SetActive(true);

        for (int i = 0; i < save.level - 1; i++)
        {
            levelsParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void RemoveCanvas()
    {
        levelsParent.parent.gameObject.SetActive(false);
    }

    public void ReplayLevel(int levelNb)
    {
        Save.SetSave(save.level,levelNb);

        var loadedLevel = levels[levelNb - 1];
        Time.timeScale = 1;

        var lines = loadedLevel.intro;

        // if there's dialogue play cutscene or else game
        if (lines == null || lines.Length < 1)
        {
            SceneManager.LoadSceneAsync("Game");
        }
        else
        {
            CutsceneManager.type = CutsceneManager.CutsceneType.Intro;
            SceneManager.LoadSceneAsync("Cutscene");
        }
    }
}
