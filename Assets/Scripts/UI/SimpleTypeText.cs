using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleTypeText : MonoBehaviour
{
    private static readonly List<char> pauses = new List<char>
        {
            '.',
            ',',
            '!',
            '?',
            '*'
        };

    TMP_Text _text;
    TMP_Text Label
    {
        get
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();

            return _text;
        }
    }
    public void Type(string fullText, Action OnTypeEndEvent, float typeSpeed = 0.05f)
    {
        gameObject.SetActive(true);
        int charCount = fullText.Length;
        int journey = 0;
        Label.maxVisibleCharacters = 0;
        Label.text = fullText;

        StartCoroutine(TypeRoutine());

        IEnumerator TypeRoutine()
        {

            while (journey <= charCount)
            {

                float pauseMultiplier = pauses.Contains(fullText[journey]) ? 5 : 1;
                yield return new WaitForSeconds(typeSpeed * pauseMultiplier);
                
                journey++;

                // Type char
                Label.maxVisibleCharacters = journey;

                if (journey >= charCount)
                    break;

            }



            OnTypeEndEvent?.Invoke();
        }
    }
}
