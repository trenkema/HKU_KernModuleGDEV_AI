using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDecision : MonoBehaviour
{
    [SerializeField] TextMeshPro decisionText;
    [SerializeField] BehaviourTree.Tree decisionTree;

    [SerializeField] Gradient foodLevelGradient;

    private void Awake()
    {
        ColorFromGradient(1f);
    }

    private void Update()
    {
        if (decisionTree != null)
            decisionText.text = decisionTree.decision;
    }

    public void ColorFromGradient(float value)  // float between 0-1
    {
        decisionText.color = foodLevelGradient.Evaluate(value);
    }
}
