﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RangeTimer
{
    public float minTimer = 0f;
    public float maxTimer = 0f;
}

public class MOMActions
{
    public Action<MovableObject> OnRemove = null;
    public Action<GameObject> OnReturnPoolManager = null;
}

public class MovableObjectManager : MonoBehaviour
{
    #region EXPOSED_FIELDS

    [SerializeField] private RangeTimer[] rangeTimerSpawn = null;
    [SerializeField] private float maxX = 0f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private PoolManager poolManager = null;

    #endregion

    #region PRIVATE_FIELDS

    private int rangeTimerIndex = 0;
    private MOMActions momActions = null;

    #endregion

    #region PROPERTIES

    public List<MovableObject> Movables { get; } = new List<MovableObject>();
    public bool SpawnActivated { get; set; } = false;
    public float BaseSpeed { get; set; } = 0f;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    #endregion

    #region UNITY_CALLS

    void Start()
    {
        InitModuleHandlers();
        BaseSpeed = speed;
    }

    void Update()
    {
        Spawn();
    }

    #endregion

    #region PUBLIC_METHODS

    public void ChangeRangeTimerSpawn(int index)
    {
        rangeTimerIndex = index < rangeTimerSpawn.Length ? 
                          index : rangeTimerSpawn.Length - 1;
    }

    #endregion

    #region PRIVATE_METHODS

    private void InitModuleHandlers()
    {
        momActions = new MOMActions();

        momActions.OnRemove += Remove;
        momActions.OnReturnPoolManager += poolManager.ReturnObjectToPool;
    }

    private void Spawn()
    {
        if (SpawnActivated)
            return;

        GameObject movableGO = poolManager.GetObjectFromPool();
        MovableObject movable = movableGO.GetComponent<MovableObject>();

        movable.InitModuleHandlers(momActions);
        movable.Speed = speed;
        movableGO.transform.position = GetRandomPosition();

        Movables.Add(movable);

        SpawnActivated = true;
        Invoke(nameof(ResetSpawnActivate), GetRandomTimerSpawn());
    }

    private void Remove(MovableObject movable)
    {
        Movables.Remove(movable);
    }

    private void ResetSpawnActivate()
    {
        SpawnActivated = false;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(pos.x - maxX, pos.x + maxX);

        return pos;
    }

    private float GetRandomTimerSpawn()
    {
        return Random.Range(rangeTimerSpawn[rangeTimerIndex].minTimer,
                            rangeTimerSpawn[rangeTimerIndex].maxTimer);
    }

    #endregion
}
