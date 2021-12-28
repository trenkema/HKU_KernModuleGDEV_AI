using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDecision : MonoBehaviour
{
    [SerializeField] TextMeshPro decisionText;
    public BehaviourTree.Tree decisionTree;

    private void Update()
    {
        if (decisionTree != null)
            decisionText.text = decisionTree.decision;
    }
}
