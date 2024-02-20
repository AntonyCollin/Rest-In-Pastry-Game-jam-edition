using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class GhostComponent : MonoBehaviour
{
    [SerializeField] private GhostData data;
    [SerializeField] private float speed;
    [SerializeField][Range(0, 100)] private float talkingChance;
    [SerializeField] private AudioClip[] voiceLines;

    private List<CookingData> order;
    private float timeBeforeLeaving;

    // put in ghostdata
    public CookingData[] foodPossibilities;
    public CookingData[] specialityPossibilities;
    public CookingData[] drinkPossibilities;
    public CookingData[] dessertPossibilities;

    public GameObject timerImg;
    public Canvas orderCanvas;


    private AudioSource audioSource;
    private Animator animator;
    private bool canBeServed = false;
    private const int nbCat = 4;
    private bool served = false;
    private bool canTalk = true;
    private int timeServed = 0;
    private const int maxNbItems = 3;
    private float timeLeft;
    private BoxCollider2D boxCollider;

    // getting assigned by the ghost manager
    [HideInInspector]
    public GhostsManager gm;
    [HideInInspector]
    public PopupManager pm;
    [HideInInspector]
    public Transform spawn;
    [HideInInspector]
    public Transform place;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public InspectionManager ins;
    [HideInInspector]
    public DialogueManager dm;


    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        order = new List<CookingData>();

        timeBeforeLeaving = data.timeBeforeLeaving;
        ChooseOrder();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canTalk)
        {
            boxCollider.enabled = false;
            float rand = Random.Range(0f, 100f);

            if (rand < talkingChance)
            {
                audioSource.PlayOneShot(voiceLines[Random.Range(0, voiceLines.Length)]);
            }

            canTalk = false;
        }
    }

    public IEnumerator GoToPlace()
    {
        while (true)
        {
            var step = speed * Time.deltaTime;

            // if going to order
            if (Vector2.Distance(transform.position, place.position) < step)
            {
                animator.SetBool("isWaiting", false);
            }

            // if sitted
            if (transform.position == place.position)
            {
                animator.SetBool("isWaiting", true);

                // when going back to spawn
                if (served)
                {
                    // return to pool, to change
                    gameObject.SetActive(false);

                    // change this, function called each time
                    if (player.nbServed == gm.ghostAmountInLvl)
                    {
                        gm.lm.NextLevel();
                    }
                }
                else
                {
                    boxCollider.enabled = true;
                    canBeServed = true;
                    StartCoroutine(Timer());
                }

                DisplayOrder();
                yield break;
            }

            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(place.position.x, place.position.y), step);
            yield return null;
        }
    }

    private IEnumerator Timer()
    {
        timeLeft = timeBeforeLeaving;
        timerImg.transform.parent.gameObject.SetActive(true);
        Image image = timerImg.GetComponent<Image>();
        image.color = Color.green;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            image.fillAmount = timeLeft / timeBeforeLeaving;

            if (image.fillAmount <= 0.5f)
            {
                image.color = Color.yellow;
            }

            if (image.fillAmount <= 0.2f)
            {
                image.color = Color.red;
            }

            yield return new WaitForEndOfFrame();
        }

        // when no time left
        if (data.ghostType != GhostData.Type.Boss)
        {
            int rand = Random.Range(0, data.missingOrderDialogues.Length);

            dm.NewDialogue(data.missingOrderDialogues[rand].line, data.missingOrderDialogues[rand].color);
        }
        GoBackToSpawn();

        // player fails one time
        player.AddStrike();

        if (player.nbFailed >= gm.failNb)
        {
            pm.OpenRetry();
        }
    }

    private void ChooseOrder()
    {
        switch (data.ghostType)
        {
            case GhostData.Type.Blue:
                int rand = Random.Range(0, 2);

                // drink or dessert
                if (rand == 0)
                {
                    RandomIngredientFromCategory(drinkPossibilities);
                }
                else
                {
                    RandomIngredientFromCategory(dessertPossibilities);
                }
                break;

            case GhostData.Type.Red:

                // random drink
                RandomIngredientFromCategory(drinkPossibilities);

                // random speciality
                RandomIngredientFromCategory(specialityPossibilities);

                // random food
                rand = Random.Range(0, 1);

                if (rand == 0)
                {
                    RandomIngredientFromCategory(foodPossibilities);
                }
                else
                {
                    RandomIngredientFromCategory(dessertPossibilities);
                }

                break;

            case GhostData.Type.Boss:
                // nb items
                rand = Random.Range(1, maxNbItems + 1);

                for (int i = 0; i <= rand - 1; i++)
                {
                    ChooseItem(Random.Range(0, nbCat));
                }
                break;

            default:

                ChooseItem(Random.Range(0, nbCat));

                break;
        }
    }

    private void RandomIngredientFromCategory(CookingData[] ingredientsFromCategory)
    {
        var rand = Random.Range(0, ingredientsFromCategory.Length);
        order.Add(ingredientsFromCategory[rand]);
    }

    // dictionnary number to cat
    private void ChooseItem(int nb)
    {
        switch (nb)
        {
            case 0:
                RandomIngredientFromCategory(foodPossibilities);
                break;
            case 1:
                RandomIngredientFromCategory(drinkPossibilities);
                break;
            case 2:
                RandomIngredientFromCategory(dessertPossibilities);
                break;
            case 3:
                RandomIngredientFromCategory(specialityPossibilities);
                break;
        }
    }

    private void DisplayOrder()
    {
        audioSource.PlayOneShot(gm.pop);
        orderCanvas.gameObject.SetActive(true);
        var panel = orderCanvas.transform.GetChild(0);
        Transform square; 

        for (int i = 0; i < order.Count; i++)
        {
            square = panel.transform.GetChild(i);
            Tooltip tooltip = square.gameObject.GetComponent<Tooltip>();
            square.gameObject.SetActive(true);
            square.GetComponent<Image>().sprite = data.ghostType == GhostData.Type.Blue ? order[i].oppositeIngredient.image : order[i].image;
            tooltip.ins = ins;

            if (data.ghostType == GhostData.Type.Blue)
            {
                tooltip.isFromBlueGhost = true;
            }

            tooltip.ingredient = order[i];
        }
    }

    public void IsOrderCorrect(CookingData[] inventory)
    {
        if (canBeServed)
        {
            foreach (var request in order)
            {
                if (!HasMatch(request, inventory))
                {
                    // error message
                    return;
                }
            }

            // if the order is correct
            OrderComplete(inventory);
        }
    }

    private void OrderComplete(CookingData[] inventory)
    {
        audioSource.Play();
        RemoveFromInventory(inventory);

        // order complete dialogue
        if (data.ghostType != GhostData.Type.Boss)
        {
            int rand = Random.Range(0, data.dialogues.Length);
            dm.NewDialogue(data.dialogues[rand].line, data.dialogues[rand].color);
        }

        GoBackToSpawn();
    }

    // clear order bubble
    private void ClearDisplay()
    {
        var panel = orderCanvas.transform.GetChild(0);

        foreach (Transform child in panel.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void GoBackToSpawn()
    {
        if (canBeServed)
        {
            timeServed++;
            StopCoroutine(Timer());
            // to fix
            timeLeft = int.MaxValue;

            // if need to order again
            if (timeServed < data.nbOfOrders)
            {
                order.Clear();
                ChooseOrder();
                ClearDisplay();
                DisplayOrder();
                Image image = timerImg.GetComponent<Image>();
                image.color = Color.green;
                timeLeft = timeBeforeLeaving;
                player.closeGhost = null;
            }
            // ghost finished ordering
            else
            {
                player.nbServed++;
                gm.ghostsRemaining--;
                player.closeGhost = null;
                GetComponent<BoxCollider2D>().enabled = false;
                place = spawn;
                gm.StopAllCoroutines();
                gm.StartCoroutine(gm.Spawning());
                StartCoroutine(GoToPlace());
                animator.SetBool("isWaiting", true);
                StopCoroutine(Timer());
                transform.parent = null;
                served = true;
                orderCanvas.gameObject.SetActive(false);
                gm.remainingGhosts.text = (int.Parse(gm.remainingGhosts.text) - 1).ToString();
                canBeServed = false;
            }
        }
    }


    private void RemoveFromInventory(CookingData[] inventory)
    {
        for (int i = 0; i <= order.Count - 1; i++)
        {
            if (HasMatch(order[i], inventory))
            {
                inventory[i] = null;
                player.RemoveImageInventory(i);
            }
        }
    }

    private bool HasMatch(CookingData request, CookingData[] inventory)
    {
        for (int i = 0; i <= inventory.Length - 1; i++)
        {
            if (inventory[i] == request)
            {
                return true;
            }
        }

        return false;
    }
}
