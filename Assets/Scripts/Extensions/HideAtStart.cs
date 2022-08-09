using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAtStart : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
