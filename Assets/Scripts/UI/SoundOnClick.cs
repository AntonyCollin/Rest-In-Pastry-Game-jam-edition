using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundOnClick : MonoBehaviour
{
    public AudioClip clickSound;
    public void Click()
    {
        GetComponent<AudioSource>().PlayOneShot(clickSound);
    }
}
