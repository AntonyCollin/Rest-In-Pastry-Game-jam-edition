using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;

public class GhostsManager : MonoBehaviour
{
    [Header("Ghosts prefab")]
    public GameObject greenGhost;
    public GameObject blueGhost;
    public GameObject redGhost;
    public GameObject boss;
    public Transform spawnZone;
    public Transform placesParent;
    public int timeBetween = 10;
    public TextMeshProUGUI remainingGhosts;
    public AudioClip pop;
    public DialogueManager dm;
    public Player player;
    public PopupManager pm;
    public LevelManager lm;
    [HideInInspector] public int ghostsRemaining;
    [HideInInspector] public int ghostAmountInLvl;
    [HideInInspector] public int failNb;

    [SerializeField] private InspectionManager ins;
    [SerializeField] private GameObject pause;
    [SerializeField] private PlayerInput esc;

    private LevelData level;
    private List<GameObject> ghosts;

    private void Start()
    {
        Time.timeScale = 1;
        lm = GetComponent<LevelManager>();
        StartLevel();
    }

    public void StartLevel()
    {
        esc.enabled = true;
        pause.gameObject.SetActive(true);
        player.GetComponent<PlayerInput>().enabled = true;
        player.ClearStrikes();
        level = lm.level;
        failNb = level.maxFailures;
        ghosts = level.ghosts.ToList();
        ghostAmountInLvl = level.ghosts.Length;
        ghostsRemaining = level.ghosts.Length;
        remainingGhosts.text = ghostsRemaining.ToString();
        player.nbServed = 0;

        // object pool
        GameObject go = Instantiate(ghosts[0], spawnZone.position, spawnZone.rotation);

        LinkGhost(go);
        InstanciateVariables(go);
        ghosts.RemoveAt(0);


        StopAllCoroutines();

        // if multiple ghosts then start spawning
        if (level.ghosts.Length > 1)
        {
            StartCoroutine(Spawning());
        }
    }

    public IEnumerator Spawning()
    {
        // while there's still pool available in the level
        while (ghostsRemaining > 0)
        {
            yield return new WaitForSeconds(timeBetween);

            if (IsPlaceAvailable())
            {
                int rand = Random.Range(0, ghosts.Count);

                // get random ghost from ghost pool of current level
                GameObject ghost = Instantiate(ghosts[rand], spawnZone.position, spawnZone.rotation);

                LinkGhost(ghost);

                InstanciateVariables(ghost);


                // remove one from pool of ghost
                ghosts.RemoveAt(rand);

                // remove pool of ghost if count 0
                //if (ghosts.Count == 0)
                //{
                //    lm.level.nextGhosts[rand] = null;
                //}
            }
            else
            {
                StopAllCoroutines();
            }
        }
        // stop couroutine, next level button then change level variable, save level number in playerprefs

    }

    private void InstanciateVariables(GameObject go)
    {
        GhostComponent gc = go.GetComponent<GhostComponent>();

        gc.StartCoroutine("GoToPlace");
        gc.gm = GetComponent<GhostsManager>();
        gc.player = player;
        gc.pm = pm;
        gc.ins = ins;
        gc.dm = dm;
    }

    private Transform AssignPlace()
    {
        foreach (Transform child in placesParent)
        {
            if (child.childCount == 0)
            {
                return child;
            }
        }

        return null;
    }

    private void LinkGhost(GameObject ghost)
    {
        var place = AssignPlace();

        ghost.transform.parent = place;
        ghost.GetComponent<GhostComponent>().spawn = spawnZone;
        ghost.GetComponent<GhostComponent>().place = place;
    }

    private bool IsPlaceAvailable()
    {
        foreach (Transform child in placesParent)
        {
            if (child.childCount == 0)
            {
                return true;
            }
        }

        return false;
    }
}

