using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject objectiveText;
    [SerializeField] GameObject objectiveSubText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            objectiveText.SetActive(!objectiveText.activeInHierarchy);
            objectiveSubText.SetActive(!objectiveSubText.activeInHierarchy);
        }
    }
}
