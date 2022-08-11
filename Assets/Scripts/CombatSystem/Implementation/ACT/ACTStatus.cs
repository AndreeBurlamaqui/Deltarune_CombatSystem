using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ACTStatus : MonoBehaviour
{

    [SerializeField] TMP_Text actNameLabel;
    public UnityEvent<int> ActSelectEvent;


    int optionID;

    public void SetupActOption(int id, string name)
    {
        gameObject.SetActive(true);
        optionID = id;
        actNameLabel.text = name;
    }

    public void SelectACT()
    {
        ActSelectEvent?.Invoke(optionID);
    }
}
