using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    public float speed = 10;
    public PopupManager cm;
    public GameObject inventoryPanel;
    public Color defaultInvColor;
    public GameObject trashCan;

    [HideInInspector] public int nbServed = 0;
    [HideInInspector] public GameObject closeGhost;
    [HideInInspector] public int nbFailed = 0;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip movedToInvSound;
    [SerializeField] private AudioClip[] footstepsSounds;
    [SerializeField] private float timeBetweenFootsteps = 1;
    [SerializeField] private GameObject strikeBoard;

    private Vector2 movementVector;
    private Rigidbody2D rb;
    private bool canInteract = false;
    private bool canServe = false;
    private GameObject interactableObject;
    private CookingData[] inventory;
    private SpriteRenderer sr;
    private Animator an;
    private float lastMovement = 1;
    private float time;
    private Transform panel;
    private Station station;
    private AudioSource stationAudio;
    private AudioSource trashAudio;
    private Animator trashAnim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventory = new CookingData[3];
        sr = GetComponent<SpriteRenderer>();
        an = GetComponent<Animator>();
        panel = inventoryPanel.transform;
        trashAudio = trashCan.GetComponent<AudioSource>();
        trashAnim = trashCan.GetComponent<Animator>();
    }

public void Move(InputAction.CallbackContext ctx)
    {
        an.SetBool("isWalking", true);
        movementVector = ctx.ReadValue<Vector2>();

        // if movement is other direction then flip sprite
        if (movementVector.x != lastMovement && movementVector.x != 0)
        {
            sr.flipX = !sr.flipX;
            lastMovement = movementVector.x;
        }

        if (ctx.canceled)
        {
            an.SetBool("isWalking", false);
        }
    }

    void FixedUpdate()
    {
        // random footstep sounds
        if (movementVector.x != 0 && time >= timeBetweenFootsteps)
        {
            time = 0;
            int rand = Random.Range(0, footstepsSounds.Length);
            audioSource.PlayOneShot(footstepsSounds[rand]);
        }

        time += Time.deltaTime;
        rb.velocity = new Vector2(movementVector.x * speed, 0);
    }

    // rework interaction system with IInteractable interface
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Station i))
        {
            canInteract = true;
            interactableObject = collision.gameObject;
            station = interactableObject.GetComponent<Station>();
            stationAudio = interactableObject.GetComponent<AudioSource>();
            cm.OpenCanvas(interactableObject.GetComponent<Station>().keyCanvas);
        }

        if (collision.TryGetComponent(out GhostComponent ghost))
        {
            canServe = true;
            closeGhost = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Station i))
        {
            canInteract = false;
            interactableObject = null;

            if (cm.IsCanvasOpen)
            {
                cm.RemoveCanvasLayer();
            }
        }

        if (collision.TryGetComponent(out GhostComponent ghost))
        {
            canServe = false;

            if (closeGhost != null)
            {
                closeGhost = null;
            }
        }
    }

    public void Interact()
    {
        if (canInteract)
        {
            if (station.interactionSound != null)
            {
                stationAudio.PlayOneShot(station.interactionSound);
            }

            cm.OpenCanvas(station.canvas.gameObject);
            station.Display();
        }
    }

    public void AddToInventory(CookingData ingredient)
    {
        for (int i = 0; i <= inventory.Length - 1; i++)
        {
            if (inventory[i] == null)
            {
                audioSource.PlayOneShot(movedToInvSound);
                inventory[i] = ingredient;
                AddImageInventory(ingredient, i);
                break;
            }
        }
    }

    public void AddImageInventory(CookingData ingredient, int idx)
    {
        Image square = panel.GetChild(idx).GetChild(0).GetComponent<Image>();
        square.color = Color.white;
        square.sprite = ingredient.image;
    }

    public void RemoveImageInventory(int idx)
    {
        Image square = panel.GetChild(idx).GetChild(0).GetComponent<Image>();
        square.sprite = null;
        square.color = defaultInvColor;
    }

    public void ClearInventory()
    {
        for (int i = 0; i <= inventory.Length - 1; i++)
        {
            RemoveImageInventory(i);
            inventory[i] = null;
        }

        trashAudio.Play();
        trashAnim.Play("New State", 0);
    }

    public void TryServeOrder()
    {
        if (canServe)
        {
            if (closeGhost != null)
            {
                var gc = closeGhost.GetComponent<GhostComponent>();
                gc.IsOrderCorrect(inventory);
            }
        }
    }

    public void AddStrike()
    {
        nbFailed += 1;
        strikeBoard.transform.GetChild(nbFailed - 1).gameObject.SetActive(true);
    }

    public void ClearStrikes()
    {
        nbFailed = 0;
        foreach (Transform child in strikeBoard.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
