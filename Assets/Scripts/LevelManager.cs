using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Canvas levelChangeCanvas;
    public Canvas settingsCanvas;
    public TextMeshProUGUI levelText;
    public LevelData[] levels;

    [HideInInspector]
    public int levelNbToLoad;
    [HideInInspector]
    public LevelData level;

    [SerializeField] private PlayerInput esc;
    [SerializeField] private PlayerInput pi;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject resume;

    private GhostsManager gm;
    private Save currentSave;

    private void Awake()
    {
        currentSave = Save.GetSave();
        // level to load 
        levelNbToLoad = currentSave.levelToReplay == 0 ? currentSave.level : currentSave.levelToReplay;
        level = levels[levelNbToLoad - 1];
        levelText.text = "Level " + levelNbToLoad.ToString();
        gm = GetComponent<GhostsManager>();
    }

    public void NextLevel()
    {
        Time.timeScale = 0;

        // if player just finished last level then check if outro otherwise end game screen
        if (levelNbToLoad + 1 > levels.Length)
        {
            var lines = levels[levelNbToLoad - 1].outro;

            if (lines != null && lines.Length > 1)
            {
                CutsceneManager.type = CutsceneManager.CutsceneType.Outro;
                SceneManager.LoadSceneAsync("Cutscene");
            }
            else
            {
                SceneManager.LoadSceneAsync("MainMenu");
                // end game screen
            }
        }

        if (levelNbToLoad + 1 <= levels.Length)
        {
            if (currentSave.levelToReplay == 0)
            {
                DisableInput();
                levelChangeCanvas.gameObject.SetActive(true);
                pause.gameObject.SetActive(false);
            }
            else
            {
                DisableInput();
                pause.gameObject.SetActive(false);
                resume.gameObject.SetActive(false);
                settingsCanvas.gameObject.SetActive(true);
            }
        }
    }

    private void DisableInput()
    {
        pi.enabled = false;
        esc.enabled = false;
    }

    public void ChangeLevelNb()
    {
        Time.timeScale = 1;
        levelChangeCanvas.gameObject.SetActive(false);
        levelNbToLoad += 1;
        level = levels[levelNbToLoad - 1];
        Time.timeScale = 1;

        var lines = levels[levelNbToLoad - 1].intro;

        if (lines != null && lines.Length > 1)
        {
            CutsceneManager.type = CutsceneManager.CutsceneType.Intro;

            SceneManager.LoadSceneAsync("Cutscene");
        }

        // if wasnt replaying a level, then save new level
        if (currentSave.levelToReplay == 0)
        {
            Save.SetSave(levelNbToLoad, 0);
            gm.StartLevel();
            levelText.text = "Level " + levelNbToLoad.ToString();
        }
    }


    // to make sure it gets saved
    private void OnApplicationQuit()
    {
        Save.SetSave(Save.GetSave().level,0);
    }

    private void OnDisable()
    {
        Save.SetSave(Save.GetSave().level, 0);
    }
}
