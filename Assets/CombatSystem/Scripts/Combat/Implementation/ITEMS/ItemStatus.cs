using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class ItemStatus : MonoBehaviour
{
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] Image selectIcon;

    public UnityEvent<int> ItemSelectEvent;

    int itemID;
    public string ItemDescription { get; private set; }

    public void SetupItemOption(int id, string name, string description)
    {
        gameObject.SetActive(true);
        itemID = id;
        ItemDescription = description;
        nameLabel.text = name;
        UnhighlightItem();
    }

    public void SelectItem() => ItemSelectEvent?.Invoke(itemID);

    public void HighlightItem()
    {
        selectIcon.DOFade(1, 0.3f).From(0);
    }

    public void UnhighlightItem()
    {
        selectIcon.DOFade(0, 0.3f).From(1);
    }
}
