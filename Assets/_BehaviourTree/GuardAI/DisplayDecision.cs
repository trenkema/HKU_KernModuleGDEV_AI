using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDecision : MonoBehaviour
{
    [SerializeField] TextMeshPro decisionText;
    public BehaviourTree.Tree decisionTree;

    [SerializeField] Color colorUnfed;
    [SerializeField] Color colorFed;
    [SerializeField] Color colorSuccessfullyFed;

    private void Awake()
    {
        ChangeColorState(0);
    }

    private void Update()
    {
        if (decisionTree != null)
            decisionText.text = decisionTree.decision;
    }

    public void ChangeColorState(int _stateIndex)
    {
        switch (_stateIndex)
        {
            case 0:
                decisionText.color = colorUnfed;
                break;
            case 1:
                decisionText.color = colorFed;
                break;
            case 2:
                decisionText.color = colorSuccessfullyFed;
                break;
        }
    }
}
