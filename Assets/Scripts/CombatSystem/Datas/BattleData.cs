using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleData", menuName = "Combat System/Battle Data")]
public class BattleData : ScriptableObject
{
    /// <summary>
    /// All characters that will be present in this battle
    /// </summary>
    public List<CharacterData> InBattleCharacters = new List<CharacterData>();


    [Tooltip("Min and max fighters range to be used on the random factor. Determines how many QTE events at the same time it'll have")]
    [SerializeField] Vector2 fightersRange = new Vector2(1, 3);

    [Tooltip("Min and max speed range to be used on the random factor. The less the faster it reachs the end of the meter")]
    [SerializeField] Vector2 cursorTimeRange = new Vector2(1, 5);

    [Tooltip("Min and max offset range to be used on the random factor. The more, the further it starts from the meter")]
    [SerializeField] Vector2 cursorOffsetRange = new Vector2(0, 50);

    /// <summary>
    /// How many protagonists (players' character) are in the battle
    /// </summary>
    public int ProtagonistCount
    {
        get
        {
            int count = InBattleCharacters.FindAll(c => c.IsProtagonist).Count;
            Debug.Log($"Finding {count} protagonists on the battle");
            return count;

        }
    }
    /// <summary>
    /// How fast in SECONDS the cursor will reach the end of the meter.
    /// </summary>
    public float RandomCursorTime => Random.Range(cursorTimeRange.x, cursorTimeRange.y);

    public float RandomOffsetRange => Random.Range(cursorOffsetRange.x, cursorOffsetRange.y);

    /// <summary>
    /// How many QTE events at the same time it'll have
    /// Max random factor will not be more than the <see cref="ProtagonistCount"/>
    /// </summary>
    public int RandomFightersCount
    {
        get
        {
            float clampMaxRange = Mathf.Clamp(fightersRange.y, 1, ProtagonistCount);
            int count = Mathf.RoundToInt(Random.Range(fightersRange.x, clampMaxRange));

            Debug.Log($"Returning {count} fighters based on unclamped max range {fightersRange.y}  vs clamped range {clampMaxRange}");
            return count;
        }
    }
}
