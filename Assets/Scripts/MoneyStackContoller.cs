using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStackContoller : MonoBehaviour
{
    [SerializeField] private float intervalValue = .2f;

    public int MaxStackCount { get; set; } = 0;
    private List<MoneyController> _moneyStack = new List<MoneyController>();
    public List<MoneyController> MoneyStack => _moneyStack;
    public bool IsFull => _moneyStack.Count >= MaxStackCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnMoney()
    {
        if (IsFull)
        {
            Debug.Log("stack dolu");
            return;
        }

        var spawnedObj = MoneyPoolManager.Instance.SpawnOnBusiness();
        spawnedObj.transform.SetParent(transform);
        spawnedObj.GetComponent<BoxCollider>().enabled = false;
        spawnedObj.transform.localPosition = _moneyStack.Count == 0 ? Vector3.zero : Vector3.up * intervalValue * _moneyStack.Count;
        spawnedObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _moneyStack.Add(spawnedObj);
    }

    public void WithdrawMoney()
    {
        PlayerMoneyHandler.Instance.CollectMoney(_moneyStack[^1]);
        _moneyStack.RemoveAt(_moneyStack.Count-1);
    }
}
