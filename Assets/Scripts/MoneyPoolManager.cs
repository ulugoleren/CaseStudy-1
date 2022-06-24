using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyPoolManager : MonoBehaviour {

    public static MoneyPoolManager Instance;
    [SerializeField] private MoneyController money;
    [SerializeField, Range(0,100)] private int initialSpawnCount;
    [SerializeField] private Vector2 spawnRangeX;
    [SerializeField] private Vector2 spawnRangeZ;
    [SerializeField] private float spawnHeight;
    [SerializeField, Range(0,5)] private float spawnCooldown;
    private float timePassed = 0f;
    private Queue<MoneyController> _moneyPool = new Queue<MoneyController>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnOvertime();
    }

    void InitialSpawn()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            var spawnedObj = Instantiate(money,transform);
            _moneyPool.Enqueue(spawnedObj);
            spawnedObj.gameObject.SetActive(false);
        }
    }

    void SpawnOvertime()
    {
        timePassed += Time.deltaTime;

        if (!(timePassed > spawnCooldown))
            return;
        timePassed = 0f;
        if (_moneyPool.Count == 0)
        {
            var spawnedObj = Instantiate(money,transform);
            _moneyPool.Enqueue(spawnedObj);
        }
        Spawn();
    }
    
    void Spawn()
    {
        var spawnedObj = _moneyPool.Dequeue();
        spawnedObj.gameObject.SetActive(true);
        spawnedObj.transform.SetParent(transform);
        spawnedObj.transform.localRotation = Quaternion.Euler(Vector3.zero);

        var randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        var randomZ = Random.Range(spawnRangeZ.x, spawnRangeZ.y);
        spawnedObj.transform.position = new Vector3(randomX, spawnHeight, randomZ);
    }
    
    public MoneyController SpawnOnBusiness()
    {
        if (_moneyPool.Count == 0)
        {
            var _spawnedObj = Instantiate(money,transform);
            _moneyPool.Enqueue(_spawnedObj);
        }
        var spawnedObj = _moneyPool.Dequeue();
        spawnedObj.gameObject.SetActive(true);
        return spawnedObj;
    }

    public void ReleaseMoney(MoneyController moneyController)
    {
        _moneyPool.Enqueue(moneyController);
    }
}
