using UnityEngine;
using UnityEngine.InputSystem;

public class PopupManager : MonoBehaviour
{
    private GameObject currentCanvas;
    public GameObject settings;
    public GameObject retry;
    public Player player;

    public void OpenCanvas(GameObject canvasToOpen)
    {
        // If the clicked canvas is already active, deactivate it
        if (currentCanvas == canvasToOpen)
        {
            RemoveCanvasLayer();
        }
        else
        {
            // If a canvas is active, deactivate it before opening the new one
            if (IsCanvasOpen)
            {
                currentCanvas.gameObject.SetActive(false);
            }

            // Activate the clicked canvas and set it as the currentCanvas
            canvasToOpen.gameObject.SetActive(true);
            currentCanvas = canvasToOpen;
        }
    }

    public void RemoveCanvasLayer()
    {
        // if canvas is open then close it
        if (IsCanvasOpen)
        {
            currentCanvas.gameObject.SetActive(false);
            currentCanvas = null;
        }
    }

    public void OpenSettings()
    {
        Time.timeScale = !settings.activeSelf ? 0 : 1;
        player.GetComponent<PlayerInput>().enabled = settings.activeSelf;
        settings.SetActive(!settings.activeSelf);
    }

    public void OpenRetry()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        Time.timeScale = 0;
        retry.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool IsCanvasOpen => currentCanvas != null;
}
