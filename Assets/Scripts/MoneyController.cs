using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyController : MonoBehaviour {

    [SerializeField] private float value;
    public float Value => value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MyReset()
    {
        GetComponent<BoxCollider>().enabled = true;
        gameObject.SetActive(false);
        MoneyPoolManager.Instance.ReleaseMoney(this);
    }
}
