using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChickenManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] TextMeshProUGUI objectiveSubText;

    [SerializeField] TextMeshProUGUI chickensSuccessfullyFedText;
    [SerializeField] TextMeshProUGUI chickensDeadText;

    [SerializeField] int amountOfTimesRequiredToFeed = 3;

    [SerializeField] int chickensSuccessfullyFedLimit = 5;
    [SerializeField] int chickensDeadLimit = 3;

    [SerializeField] int amountOfChickenDrops;

    [SerializeField] List<GameObject> chickenDrops = new List<GameObject>();

    private int chickensSuccessfullyFed = 0;
    private int chickensDead = 0;

    private int amountOfChickenDropsSelected = 0;

    private void Start()
    {
        EventSystemNew.Subscribe(Event_Type.CHICKEN_DIED, ChickenDied);
        EventSystemNew.Subscribe(Event_Type.CHICKEN_SUCCESSFULLY_FED, ChickenSuccessfullyFed);

        objectiveText.text = "Feed " + chickensSuccessfullyFedLimit + " Chickens " + amountOfTimesRequiredToFeed + " Times";
        objectiveSubText.text = "Only " + chickensDeadLimit + " Chickens Can Die";

        chickensSuccessfullyFedText.text = chickensSuccessfullyFed + " Chickens Fed";
        chickensDeadText.text = chickensDead + " Chickens Dead";

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
                chickenDrops.Remove(chickenDrops[randomNumber]);

                amountOfChickenDropsSelected++;
            }
        }
    }

    private void ChickenSuccessfullyFed()
    {
        chickensSuccessfullyFed++;

        chickensSuccessfullyFedText.text = chickensSuccessfullyFed + " Chickens Fed";

        if (chickensSuccessfullyFed >= chickensSuccessfullyFedLimit)
        {
            EventSystemNew.RaiseEvent(Event_Type.GAME_WON);
        }
    }

    private void ChickenDied()
    {
        chickensDead++;

        chickensDeadText.text = chickensDead + " Chickens Dead";

        if (chickensDead >= chickensDeadLimit)
        {
            EventSystemNew.RaiseEvent(Event_Type.GAME_LOST);
        }
    }

    public int GetChickensFed()
    {
        return chickensSuccessfullyFed;
    }

    public int GetChickensDied()
    {
        return chickensDead;
    }
}
