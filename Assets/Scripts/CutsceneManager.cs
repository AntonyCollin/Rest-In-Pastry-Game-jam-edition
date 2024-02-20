using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneType type;
    public enum CutsceneType { Intro, Fail, Outro }
    public LevelData[] levels;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI dialogueText;
    public RectTransform leftPos;
    public RectTransform rightPos;
    public RectTransform centerPos;
    public GameObject nameBox;
    public float cooldownBetweenClicks = 1;

    [SerializeField] private RectTransform lowerCenter;
    [SerializeField] private Image bg;

    private LevelData loadedLevel;
    private int currentLine = 0;
    private DialogueLine[] lines;
    private DialogueLine line;
    private bool canClick = true;
    private bool cutsceneFinished = false;
    private Save save;

    private void Awake()
    {
        save = Save.GetSave();

        int levelNb = save.levelToReplay == 0 ? save.level : save.levelToReplay;
        loadedLevel = levels[levelNb - 1];
        Time.timeScale = 1;

        switch (type)
        {
            case CutsceneType.Intro:
                lines = loadedLevel.intro;
                break;

                // for failure dialogues -- maybe in the future
            case CutsceneType.Fail:

                break;

            case CutsceneType.Outro:
                lines = loadedLevel.outro;

                break;
        }

        // play first line
        PlayLine();
    }

    // play line is called on click
    public void PlayLine()
    {
        // if cooldown between clicks is done
        if (canClick)
        {
            if (cutsceneFinished)
            {
                if (type == CutsceneType.Outro)
                {
                    SceneManager.LoadSceneAsync("Credits");
                }
                else
                {
                    SceneManager.LoadSceneAsync("Game");
                }
            }
            else
            {
                StartCoroutine(Cooldown());

                canClick = false;
                line = lines[currentLine];

                TryChangeBackground();
                TryRedrawScene();

                nameBox.gameObject.SetActive(line.speakerName.Length > 1 && line.speakerName != "" && line.speakerName != null);
                charName.text = line.speakerName;
                dialogueText.text = line.line;

                // if not a character speaking, display it in italic
                if (!nameBox.activeSelf)
                {
                    dialogueText.fontStyle = FontStyles.Italic;
                }
                else
                {
                    dialogueText.fontStyle = FontStyles.Normal;
                }

                if (line.isLastLine)
                {
                    cutsceneFinished = true;
                }

                currentLine += 1;
            }
        }
    }
    private IEnumerator Cooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldownBetweenClicks);
            canClick = true;
            yield break;
        }
    }
    private void TryChangeBackground()
    {
        if (lines[currentLine].background != null)
        {
            bg.color = Color.white;
            bg.sprite = lines[currentLine].background;
        }
    }
    private void TryRedrawScene()
    {
        if (line.spritePositions != null && line.spritePositions.Length >= 1)
        {
            DestroySprites();
            PlaceSprites();
        }
    }
    private void PlaceSprites()
    {
        // depending on their position display sprites at different rotation
        for (int i = 0; i <= line.spritePositions.Length - 1; i++)
        {
            switch (line.spritePositions[i])
            {
                case DialogueLine.Position.Left:
                    ChangeSprite(leftPos, line.sprites[i], new Quaternion(0, -180, 0, 0));
                    break;
                case DialogueLine.Position.Right:
                    ChangeSprite(rightPos, line.sprites[i], new Quaternion(0, 0, 0, 0));
                    break;
                case DialogueLine.Position.Center:
                    ChangeSprite(centerPos, line.sprites[i], new Quaternion(0, -180, 0, 0));
                    break;
                case DialogueLine.Position.LowerCenter:
                    ChangeSprite(lowerCenter, line.sprites[i], new Quaternion(0, -180, 0, 0));
                    break;
            }
        }
    }
    private void ChangeSprite(RectTransform pos, Sprite newSprite, Quaternion rotation)
    {
        pos.gameObject.SetActive(true);
        pos.GetComponent<Image>().color = Color.white;
        pos.GetComponent<Image>().sprite = newSprite;
        pos.rotation = rotation;
    }
    private void ResetSprites(RectTransform pos)
    {
        pos.gameObject.SetActive(false);
        pos.rotation = new Quaternion(0, 0, 0, 0);
    }
    private void DestroySprites()
    {
        ResetSprites(centerPos);
        ResetSprites(rightPos);
        ResetSprites(leftPos);
        ResetSprites(lowerCenter);
    }

}