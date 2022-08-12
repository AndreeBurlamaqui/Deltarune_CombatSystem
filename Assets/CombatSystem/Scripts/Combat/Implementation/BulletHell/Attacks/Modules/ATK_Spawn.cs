using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "NewSpawnModule", menuName = "Combat System/Enemy Attack/New Spawn Position Module")]
public class ATK_Spawn : ScriptableObject
{
    [Help("[SPAWN MODULE]: \n\n" +
        "HOW IT WORKS: \n" +
        "> Attach the target attack prefab." +
        "> Select the spawn type" +
        "> If spawn type is related to another prefab, assign the spawnable area at _spawnSetterPrefab" +
        "> If spawn type is related to be around origin, set a radius range" +
        "\n\n" +
        "TIP: Make the spawn setter prefab as a child of the ring so you'll always have the position related to that." +
        "Also always make the anchor setted as middle-center")]
    [SerializeField] GameObject _attackVisualPrefab;
    GameObject _runtimeVisual;
    GameObject _runtimeSpawner;

    [SerializeField] SpawnType _spawnType;
    [SerializeField] GameObject _spawnSetterPrefab;
    [SerializeField] float _radiusRange;
    [SerializeField] bool isUnique;

    public GameObject RuntimeGameObject => _runtimeVisual;
    public Transform RuntimeTransform => _runtimeVisual?.transform;

    public void SpawnSpawner(BulletHellHandler bulletHell)
    {
        if(!isUnique || _runtimeSpawner == null)
            _runtimeSpawner = Instantiate(_spawnSetterPrefab, bulletHell.transform);

    }

    public GameObject SpawnAttack(BulletHellHandler bulletHell)
    {
        // Instantiate bullet
        _runtimeVisual = Instantiate(_attackVisualPrefab, bulletHell.transform);



        RuntimeTransform.localPosition = SpawnByType();

        return _runtimeVisual;

        Vector3 SpawnByType()
        {
            return _spawnType switch
            {
                SpawnType.AtAttack => SpawnAtPrefabAttack(),
                SpawnType.AtSpawner => SpawnAtPrefabSpawner(),
                SpawnType.AtRuntimeSpawner=> SpawnAtRuntimeSpawner(),
                SpawnType.RandomAroundSelfOrigin => SpawnAtAroundSelf(),
                SpawnType.RandomAroundSpawnerOrigin => SpawnAroundPrefab(),
                SpawnType.RandomInsideSpawner => SpawnInsidePrefab(),
                _ => throw new System.NotImplementedException(),
            };
        }

    }

    public void DespawnAttack(BulletHellHandler bulletHell, GameObject toDespawn, float delay)
    {
        if (toDespawn == null)
            return;

        //bulletHell.currentBullets.Remove(toDespawn.transform);
        toDespawn.transform.DOKill();

        Destroy(toDespawn, delay);

        //_runtimeVisual = null;
    }

    public void DespawnSpawner()
    {
        if (_runtimeSpawner != null)
            Destroy(_runtimeSpawner);
    }



    private Vector3 SpawnAtPrefabAttack()
    {
        return _attackVisualPrefab.transform.position;
    }

    private Vector3 SpawnAtPrefabSpawner()
    {
        Debug.Log("Spawning at prefab spawner");
        return _spawnSetterPrefab.transform.position;
    }

    private Vector3 SpawnAtAroundSelf()
    {
        return _attackVisualPrefab.transform.position + (Vector3)Random.insideUnitCircle * (_radiusRange * 10);
    }

    private Vector3 SpawnAroundPrefab()
    {
        return _spawnSetterPrefab.transform.position + (Vector3)Random.insideUnitCircle * (_radiusRange * 10);
    }

    private Vector3 SpawnInsidePrefab()
    {
        // Get Rect Transform
        RectTransform rect = _spawnSetterPrefab.GetComponent<RectTransform>();

        return new Vector3(Random.Range(rect.offsetMax.x, rect.offsetMin.x), Random.Range(rect.offsetMax.y, rect.offsetMin.y));
    }

    private Vector3 SpawnAtRuntimeSpawner()
    {
        return _runtimeSpawner != null ? _runtimeSpawner.transform.localPosition : SpawnAtPrefabSpawner();
    }



    public enum SpawnType
    {
        /// <summary>
        /// Spawn the attack at the origin point of the attack's prefab.
        /// </summary>
        AtAttack,

        /// <summary>
        /// Spawn the attack at <see cref="_spawnSetterPrefab"/> origin
        /// </summary>
        AtSpawner,

        /// <summary>
        /// Spawn the attack at <see cref="_runtimeSpawner"/> position
        /// </summary>
        AtRuntimeSpawner,

        /// <summary>
        /// Spawn the attack around it's origin randomly
        /// </summary>
        RandomAroundSelfOrigin,

        /// <summary>
        /// Spawn the attack around prefab's origin randomly
        /// </summary>
        RandomAroundSpawnerOrigin,

        /// <summary>
        /// Spawn the attack in a random position inside a prefab
        /// </summary>
        RandomInsideSpawner
    }
}
