using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

	[SerializeField] private Transform moveTarget;
	[SerializeField] private float moveDuration;
	[SerializeField] private AnimationCurve moveCurve;
	[SerializeField] private AnimationCurve heightCurveUp;
	[SerializeField] private AnimationCurve heightCurveDown;

	[ContextMenu("ParabolicMovement")]
	public void PerformParabolicMovement()
	{
		var parabolicMovementCoroutine = ParabolicMovement(transform.position, moveTarget.position, moveDuration, moveCurve);
		StartCoroutine(parabolicMovementCoroutine);
	}

	IEnumerator ParabolicMovement(Vector3 startPos, Vector3 targetPos, float duration, AnimationCurve _moveCurve)
	{
		var init = startPos;
		var target = targetPos;
		var passed = 0f;

		while (passed < duration)
		{
			passed += Time.deltaTime;
			var normalized = passed / duration;
			var moveEvaluated = _moveCurve.Evaluate(normalized);
			var heightEvaluated = startPos.y < targetPos.y ? heightCurveUp.Evaluate(normalized) : heightCurveDown.Evaluate(normalized);

			var currentPos = Vector3.Lerp(startPos, targetPos, moveEvaluated);
			var currentHeight = Mathf.LerpUnclamped(startPos.y, targetPos.y, heightEvaluated);
			currentPos.y = currentHeight;
			transform.position = currentPos;


			yield return null;
		}
	}
}