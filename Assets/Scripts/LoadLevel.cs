using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public LevelData[] levels;
    private Save save;

    private DialogueLine[] lines;

    private void Awake()
    {
        save = Save.GetSave();
    }

    public void LoadLevel()
    {
        int levelNb = save.levelToReplay == 0 ? save.level : save.levelToReplay;
        var loadedLevel = levels[levelNb - 1];
        Time.timeScale = 1;

        lines = loadedLevel.intro;

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
