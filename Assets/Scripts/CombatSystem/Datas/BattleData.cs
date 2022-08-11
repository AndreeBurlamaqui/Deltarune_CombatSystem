using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCacto.Utils;

[CreateAssetMenu(fileName = "BattleData", menuName = "Combat System/Battle Data")]
public class BattleData : ScriptableObject
{

    [Header("CHARACTERS")]

    /// <summary>
    /// All characters that will be present in this battle
    /// </summary>
    public List<CharacterData> InBattleCharacters = new();

    /// <summary>
    /// Every protagonists that are in the current battle.
    /// <para>----</para>
    /// Key is <see cref="CharacterData.battleID"/>
    /// </summary>
    public Dictionary<int, ProtagonistData> InBattleProtagonists = new();

    /// <summary>
    /// Every enemies that are in the current battle.
    /// <para>----</para>
    /// Key is <see cref="CharacterData.battleID"/>
    /// </summary>
    public Dictionary<int, EnemyData> InBattleEnemies = new();

    [Header("FIGHT OPTION")]

    [Tooltip("Min and max fighters range to be used on the random factor. Determines how many QTE events at the same time it'll have")]
    [SerializeField] FloatRange fightersRange = new FloatRange(1, 3);

    [Tooltip("Min and max speed range to be used on the random factor. The less the faster it reachs the end of the meter")]
    [SerializeField] FloatRange cursorTimeRange = new FloatRange(1, 5);

    [Tooltip("Min and max offset range to be used on the random factor. The more, the further it starts from the meter")]
    [SerializeField] FloatRange cursorOffsetRange = new FloatRange(0, 50);

    [Header("ACT OPTION")]
    public ActOptions actOptions;

    #region PROPERTIES

    /// <summary>
    /// How fast in SECONDS the cursor will reach the end of the meter.
    /// </summary>
    public float RandomCursorTime => cursorTimeRange.RandomInRange;

    public float RandomOffsetRange => cursorOffsetRange.RandomInRange;

    /// <summary>
    /// How many QTE events at the same time it'll have
    /// Max random factor will not be more than the <see cref="ProtagonistCount"/>
    /// </summary>
    public int RandomFightersCount => fightersRange.RandomMinByCustomMax(InBattleProtagonists.Count).ToInt();

    public int TotalEnemies
    {
        get
        {
            int enemyCount = 0;

            foreach(EnemyData ed in InBattleEnemies.Values)
                foreach (Transform t in ed.RuntimeTransform)
                    enemyCount++;

            return enemyCount;
        }
    }

    #endregion

    public void ClearBattleRuntimeInformations()
    {
        InBattleCharacters.Clear();
        InBattleEnemies.Clear();
        InBattleProtagonists.Clear();
    }
}

public enum BattleState
{
    /// <summary>
    /// Battle has just started.
    /// </summary>
    Start,

    /// <summary>
    /// Player turn to act.
    /// <para>This means that one of the protagonists will have the options enabled.</para>
    /// </summary>
    PlayerTurn,

    /// <summary>
    /// System is waiting for other turn.
    /// </summary>
    Waiting,

    /// <summary>
    /// Enemy turn, this means that we're under the bullet hell.
    /// </summary>
    EnemyTurn,

    /// <summary>
    /// Battle ended.
    /// </summary>
    End
}
