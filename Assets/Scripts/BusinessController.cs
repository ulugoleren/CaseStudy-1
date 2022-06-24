using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BusinessController : MonoBehaviour {

	[SerializeField] private float price;
	[SerializeField] private TextMeshPro priceText;
	[SerializeField] private Transform centerPoint;
	[SerializeField] private List<MoneyStackContoller> moneyStacks;
	[SerializeField, Range(1, 50)] private int MaxStackCount;
	[SerializeField] private float moneyGenerateCooldown;
	[SerializeField] private float withdrawDuration;
	[SerializeField] private Color purchasedColor;
	private float passedTimeGenerate = 0f;
	private float passedTimeWithdraw = 0f;
	public Transform CenterPoint => centerPoint;
	public bool IsPurchased { get; set; } = false;
	public bool WithdrawProcess { get; set; } = false;
	private float restNeededMoney;
	// Start is called before the first frame update
	void Start()
	{
		restNeededMoney = price;
		UpdateText(restNeededMoney);

		foreach (var moneyStack in moneyStacks)
		{
			moneyStack.MaxStackCount = MaxStackCount;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(!IsPurchased)
			return;
		
		GenerateMoney();
		WithdrawMoney();
	}

	public void EvaluateMoney(float value)
	{
		if (IsPurchased)
			return;
		restNeededMoney -= value;
		if (Math.Abs(restNeededMoney - 0f) < 0.1)
		{
			IsPurchased = true;
			priceText.text = "PURCHASED";
			WithdrawProcess = true;
			GetComponent<MeshRenderer>().material.color = purchasedColor;
			return;
		}


		UpdateText(restNeededMoney);
	}

	void UpdateText(float value)
	{
		priceText.text = value.ToString() + " $";
	}

	void GenerateMoney()
	{
		if(moneyStacks[^1].IsFull)
			return;
		
		passedTimeGenerate += Time.deltaTime;

		if (passedTimeGenerate > moneyGenerateCooldown)
		{
			passedTimeGenerate = 0f;

			for (int i = 0; i < moneyStacks.Count; i++)
			{
				if (moneyStacks[i].IsFull)
				{
					continue;
				}
				
				moneyStacks[i].SpawnMoney();
				break;
			}
		}
	}

	void WithdrawMoney()
	{
		
		if(moneyStacks[0].MoneyStack.Count == 0 || !WithdrawProcess)
			return;
		
		passedTimeWithdraw += Time.deltaTime;

		if (passedTimeWithdraw > withdrawDuration)
		{
			passedTimeWithdraw = 0f;

			for (int i = moneyStacks.Count-1 ; i >= 0; i--)
			{
				if (moneyStacks[i].MoneyStack.Count == 0)
				{
					continue;
				}
				
				moneyStacks[i].WithdrawMoney();
				break;
			}
		}
	}
}