using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TinyCacto.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class TinyText : MonoBehaviour
    {
        [Tooltip("Default type speed as in when the request doesn't override the typeSpeed value. \n Measured in seconds.")]
        [SerializeField] float defaultTypeSpeed = 0.05f;

        [Tooltip("Multiplied delay when a ponctuaction (defined in script) is typed. \n Make things better to be read.")]
        [SerializeField] float defaultPauseSpeed = 5;

        private static readonly List<char> pauses = new List<char>
        {
            '.',
            ',',
            '!',
            '?',
            '*'
        };

        TMP_Text _text;
        public TMP_Text Label
        {
            get
            {
                if (_text == null)
                    _text = GetComponent<TMP_Text>();

                return _text;
            }
        }

        /// <summary>
        /// A simple text typer.
        /// </summary>
        /// <param name="fullText">Send the whole text you want to display, if it overflows, the script will auto split it</param>
        /// <param name="OnTypeEndEvent">Action callback when text ends</param>
        /// <param name="typeSpeed">Type speed, default of <see cref="defaultPauseSpeed"/></param>
        /// <returns>The coroutine. Useful in case you want to type and wait in a coroutine</returns>
        public float Type(string fullText, Action OnTypeEndEvent = null, float typeSpeed = 0)
        {
            // Set gameobject active otherwise we'll get an error from trying to play the coroutine
            gameObject.SetActive(true);
            Label.gameObject.SetActive(true);

            // Initial journey value
            int journey = 0;

            if (typeSpeed <= 0) // Use default speed
                typeSpeed = defaultTypeSpeed;

            // Set visible characters to 0 to simulate the type effect
            Label.maxVisibleCharacters = 0;

            // Then write whatever we want so even on super low fps we don't get anything wrong
            Label.text = fullText;


            StartCoroutine(TypeRoutine());

            // return time elapsed to finish it
            
            float timeToType = (fullText.Length + defaultPauseSpeed + (fullText.Count(x => pauses.Contains(x)) * defaultPauseSpeed)) * typeSpeed;
            Debug.Log($"Tiny type will take {timeToType} seconds to type the full given text");
            return timeToType;

            IEnumerator TypeRoutine()
            {
                // To sum up
                // Each (typeSpeed * pauseMultiplier) seconds we'll go to another loop
                // Each loop we increase the maxVisibleCharacters in one
                // Loop is forcefully broke when the journey goes above the charCount so we avoid any non-sense error

                // Wait one frame to update the text mesh
                yield return new WaitForEndOfFrame();

                // So that we know if it's overflowing
                bool isOverflowing = Label.isTextOverflowing;

                // Get the char count
                // If is overflowing, add 1 to count empty spaces when splitting
                int charCount = isOverflowing ? Label.firstOverflowCharacterIndex + 1 : fullText.Length;
                string currentText = isOverflowing ? fullText.Substring(0, charCount) : fullText;

                if (isOverflowing)
                {
                    Label.text = currentText; // Write again

                    // Wait one frame to update the text mesh
                    yield return new WaitForEndOfFrame();
                }

                while (journey <= charCount)
                {

                    if (journey >= charCount)
                        break;

                    // To know if we should give a little delay check the char we're going to type
                    // by getting the current journey value in the text string
                    float pauseMultiplier = pauses.Contains(currentText[journey]) ? defaultPauseSpeed : 1;

                    yield return new WaitForSeconds(typeSpeed * pauseMultiplier);

                    journey++;

                    // Type char
                    Label.maxVisibleCharacters = journey;
                }


                // Give the player some time to read the current text
                yield return new WaitForSeconds(defaultPauseSpeed);

                if (isOverflowing)
                {
                    // Split the text into another routine when this one ends
                    // Instead of invoking the action

                    string leftText = fullText.Substring(journey - 1);
                    Debug.Log("Text was overflowing, typing the rest [...]" + leftText);


                    Type(leftText, OnTypeEndEvent, typeSpeed);
                }
                else
                {
                    // Invoke the action in case the caller wants to know when the typing is over
                    OnTypeEndEvent?.Invoke();
                }
            }


        }
    }
}