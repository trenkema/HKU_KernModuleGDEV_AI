using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChickenManager : MonoBehaviour
{
    [SerializeField] float timeToDropNewFruit = 2.5f;

    [SerializeField] float timeToKeepChickensAlive = 300f;

    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] TextMeshProUGUI objectiveSubText;

    [SerializeField] TextMeshProUGUI chickensDeadText;

    [SerializeField] int chickensDeadLimit = 3;

    [SerializeField] int amountOfChickenDrops;

    [Header("References")]
    [SerializeField] List<GameObject> chickenDrops = new List<GameObject>();

    [SerializeField] List<Transform> chickenDropsSpawnpoints = new List<Transform>();

    [Space(5)]

    [SerializeField] List<Transform> chickenSpawnpoints = new List<Transform>();

    [SerializeField] GameObject[] chickens;

    private Dictionary<GameObject, Transform> droppedFruits = new Dictionary<GameObject, Transform>();

    private float keepChickensAliveTimer = 0;

    private int chickensFed = 0;
    private int chickensDead = 0;

    private int amountOfChickenDropsSelected = 0;

    private bool hasStarted = false;

    private void Awake()
    {
        keepChickensAliveTimer = timeToKeepChickensAlive;

        for (int i = 0; i < chickens.Length; i++)
        {
            int randomSpawnpoint = Random.Range(0, chickenSpawnpoints.Count);

            chickens[i].transform.position = chickenSpawnpoints[randomSpawnpoint].position;

            chickenSpawnpoints.Remove(chickenSpawnpoints[randomSpawnpoint]);
        }
    }

    private void Start()
    {
        EventSystemNew.Subscribe(Event_Type.CHICKEN_DIED, ChickenDied);
        EventSystemNew.Subscribe(Event_Type.START_GAME, StartGame);
        EventSystemNew<GameObject>.Subscribe(Event_Type.FRUIT_EATEN, FruitEaten);

        string timeLeft = FormatTime(keepChickensAliveTimer);

        objectiveText.text = "Keep The Chickens Alive For " + timeLeft;

        objectiveSubText.text = "Only " + chickensDeadLimit + " Chicken(s) Can Die";

        chickensDeadText.text = chickensDead + " Chicken(s) Died";

        foreach (var chickenDrop in chickenDrops)
        {
            chickenDrop.SetActive(false);
        }

        for (int i = 0; i < amountOfChickenDrops; i++)
        {
            if (amountOfChickenDropsSelected <= amountOfChickenDrops)
            {
                int randomNumber = Random.Range(0, chickenDrops.Count);

                chickenDrops[randomNumber].SetActive(true);

                int randomPosition = Random.Range(0, chickenDropsSpawnpoints.Count);

                chickenDrops[randomNumber].transform.position = chickenDropsSpawnpoints[randomPosition].position;

                droppedFruits.Add(chickenDrops[randomNumber], chickenDropsSpawnpoints[randomPosition]);

                chickenDrops.Remove(chickenDrops[randomNumber]);
                chickenDropsSpawnpoints.Remove(chickenDropsSpawnpoints[randomPosition]);

                amountOfChickenDropsSelected++;
            }
        }
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.CHICKEN_DIED, ChickenDied);
        EventSystemNew.Unsubscribe(Event_Type.START_GAME, StartGame);
        EventSystemNew<GameObject>.Unsubscribe(Event_Type.FRUIT_EATEN, FruitEaten);

        CancelInvoke();
    }

    private void Update()
    {
        if (hasStarted)
        {
            keepChickensAliveTimer -= Time.deltaTime;

            string timeLeft = FormatTime(keepChickensAliveTimer);

            objectiveText.text = "Keep The Chickens Alive For " + timeLeft;

            if (keepChickensAliveTimer <= 0)
            {
                EventSystemNew.RaiseEvent(Event_Type.GAME_WON);
            }
        }
    }

    private void FruitEaten(GameObject _fruitEaten)
    {
        chickenDrops.Add(_fruitEaten);
        chickenDropsSpawnpoints.Add(droppedFruits[_fruitEaten]);

        droppedFruits.Remove(_fruitEaten);

        amountOfChickenDropsSelected--;

        chickensFed++;

        Invoke("SpawnFruit", timeToDropNewFruit);
    }

    private void SpawnFruit()
    {
        for (int i = 0; i < amountOfChickenDrops; i++)
        {
            if (amountOfChickenDropsSelected <= amountOfChickenDrops)
            {
                int randomNumber = Random.Range(0, chickenDrops.Count);

                chickenDrops[randomNumber].SetActive(true);

                int randomPosition = Random.Range(0, chickenDropsSpawnpoints.Count);

                chickenDrops[randomNumber].transform.position = chickenDropsSpawnpoints[randomPosition].position;

                droppedFruits.Add(chickenDrops[randomNumber], chickenDropsSpawnpoints[randomPosition]);

                chickenDrops.Remove(chickenDrops[randomNumber]);
                chickenDropsSpawnpoints.Remove(chickenDropsSpawnpoints[randomPosition]);

                amountOfChickenDropsSelected++;
            }
            else
                break;
        }
    }

    private void StartGame()
    {
        hasStarted = true;
    }

    private string FormatTime(float _time)
    {
        int minutes = (int)_time / 60;
        int seconds = (int)_time - 60 * minutes;

        return (minutes + "m " + seconds + "s");
    }

    private void ChickenDied()
    {
        chickensDead++;

        chickensDeadText.text = chickensDead + " Chicken(s) Died";

        if (chickensDead >= chickensDeadLimit)
        {
            EventSystemNew.RaiseEvent(Event_Type.GAME_LOST);
        }
    }

    public int GetChickensFed()
    {
        return chickensFed;
    }

    public int GetChickensDied()
    {
        return chickensDead;
    }
}
