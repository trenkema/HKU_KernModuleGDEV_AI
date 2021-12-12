using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDecision : MonoBehaviour
{
    [SerializeField] TextMeshPro decisionText;
    [SerializeField] GuardBT guard;
    [SerializeField] AllyBT ally;

    private void Update()
    {
        if (guard != null)
            decisionText.text = guard.getDecision;

        if (ally != null)
            decisionText.text = ally.getDecision;
    }
}
