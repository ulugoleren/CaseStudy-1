using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMoneyHandler : MonoBehaviour {

	public static PlayerMoneyHandler Instance;
	[SerializeField] private Transform moneyStackPivot;
	[SerializeField] private float intervalValue;
	[SerializeField] private AnimationCurve moveCurve;
	[SerializeField] private AnimationCurve heightCurveUp;
	[SerializeField] private AnimationCurve heightCurveDown;
	[SerializeField] private float moveDuration;
	[SerializeField] private float depositCooldown;
	private BusinessController currentBusiness;
	private bool depositProcess = false;
	private float depositTimePassed = 0f;
	private bool enteredSaleArea = false;

	private List<MoneyController> _moneyStack = new List<MoneyController>();

	private void Awake()
	{
		Instance = this;
	}
	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update()
	{
		DepositProcess();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<MoneyController>(out var moneyController))
		{
			// other.transform.SetParent(transform);
			// var targetPos = _moneyStack.Count == 0 ? moneyStackPivot.localPosition : moneyStackPivot.localPosition + Vector3.up * intervalValue * _moneyStack.Count;
			// StartCoroutine(ParabolicMovement(other.transform, targetPos, moveDuration, moveCurve));
			// other.transform.DOLocalRotate(Vector3.zero, moveDuration);
			//
			// _moneyStack.Add(moneyController);
			
			CollectMoney(moneyController);
		}
		
		if(enteredSaleArea)
			return;
		
		if (other.CompareTag("SaleArea"))
		{
			enteredSaleArea = true;
			currentBusiness = other.GetComponentInParent<BusinessController>();
			if (currentBusiness.IsPurchased)
			{
				currentBusiness.WithdrawProcess = true;
				return;
			}
				
			depositProcess = true;
			depositTimePassed = 0f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(!enteredSaleArea)
			return;

		if (other.CompareTag("SaleArea"))
		{
			enteredSaleArea = false;
			if (currentBusiness.IsPurchased)
			{
				currentBusiness.WithdrawProcess = false;
			}

			depositCooldown = .2f;
			depositProcess = false;
			currentBusiness = null;
		}
	}

	void DepositProcess()
	{
		if (!currentBusiness || currentBusiness.IsPurchased)
			return;

		depositTimePassed += Time.deltaTime;

		if (depositTimePassed < depositCooldown)
			return;

		if (_moneyStack.Count == 0)
			return;
		depositTimePassed = 0f;
		var targetMoney = _moneyStack[^1];
		targetMoney.GetComponent<BoxCollider>().enabled = false;
		targetMoney.transform.SetParent(currentBusiness.CenterPoint);
		StartCoroutine(ParabolicMovement(targetMoney.transform, Vector3.zero, moveDuration, moveCurve));
		targetMoney.transform.DOLocalRotate(Vector3.zero, moveDuration);
		depositCooldown *= .9f;
		//depositCooldown = Mathf.Clamp(depositCooldown, .05f, .2f);

		_moneyStack.Remove(targetMoney);
		StartCoroutine(DepositOnFinish(targetMoney, currentBusiness, moveDuration));
	}

	public void CollectMoney(MoneyController moneyController)
	{
		moneyController.transform.SetParent(transform);
		var targetPos = _moneyStack.Count == 0 ? moneyStackPivot.localPosition : moneyStackPivot.localPosition + Vector3.up * intervalValue * _moneyStack.Count;
		StartCoroutine(ParabolicMovement(moneyController.transform.transform, targetPos, moveDuration, moveCurve));
		moneyController.transform.DOLocalRotate(Vector3.zero, moveDuration);

		_moneyStack.Add(moneyController);
	}

	IEnumerator DepositOnFinish(MoneyController moneyController,BusinessController businessController, float delay)
	{
		yield return new WaitForSeconds(delay);
		businessController.EvaluateMoney(moneyController.Value);
		moneyController.MyReset();
	}

	IEnumerator ParabolicMovement(Transform moveTarget, Vector3 targetPos, float duration, AnimationCurve _moveCurve)
	{
		var init = moveTarget.localPosition;

		var passed = 0f;

		while (passed < duration)
		{
			passed += Time.deltaTime;
			var normalized = passed / duration;
			var moveEvaluated = _moveCurve.Evaluate(normalized);
			var heightEvaluated = init.y < targetPos.y ? heightCurveUp.Evaluate(normalized) : heightCurveDown.Evaluate(normalized);

			var currentPos = Vector3.Lerp(init, targetPos, moveEvaluated);
			var currentHeight = Mathf.LerpUnclamped(init.y, targetPos.y, heightEvaluated);
			currentPos.y = currentHeight;
			moveTarget.localPosition = currentPos;


			yield return null;
		}
	}
}