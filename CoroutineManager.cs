using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : BaseManager<CoroutineManager> {

	public static Coroutine StartCoroutine(IEnumerator ie, MonoBehaviour behaviour = null)
	{
		return (behaviour ?? (MonoBehaviour) Instance).StartCoroutine(ie);
	}

	public static void StopCoroutine(IEnumerator ie, MonoBehaviour behaviour = null)
	{
		if (behaviour || Instance)
		{
			(behaviour ?? (MonoBehaviour) Instance).StopCoroutine(ie);
		}
	}

	#region delay

	public static Coroutine EndOfFrameOperation(System.Action operation, MonoBehaviour behaviour = null)
	{
		return DelayOperation(new WaitForEndOfFrame(), operation, behaviour);
	}

	public static Coroutine DelayOperation(float delay, System.Action operation, MonoBehaviour behaviour = null)
	{
		return DelayOperation(new WaitForSeconds(delay), operation, behaviour);
	}

	public static Coroutine DelayOperation(YieldInstruction delay, System.Action operation, MonoBehaviour behaviour = null)
	{
		return StartCoroutine(DoDelayOperation(delay, operation), behaviour);
	}

	private static IEnumerator DoDelayOperation(YieldInstruction delay, System.Action operation)
	{
		if (operation != null)
		{
			yield return delay;
			operation();
		}
	}

	#endregion

	#region loop

	public static Coroutine LoopOperation(System.Func<float, bool> whileFunc, System.Action operation, MonoBehaviour behaviour = null)
	{
		return LoopOperation(whileFunc, null, operation, behaviour);
	}

	public static Coroutine LoopOperation(System.Func<float, bool> whileFunc, float interval, System.Action operation, MonoBehaviour behaviour = null)
	{
		return LoopOperation(whileFunc, new WaitForSeconds(interval), operation, behaviour);
	}

	public static Coroutine LoopOperation(System.Func<float, bool> whileFunc, YieldInstruction interval, System.Action operation, MonoBehaviour behaviour = null)
	{
		return StartCoroutine(DoLoopOperation(whileFunc, interval, operation), behaviour);
	}

	private static IEnumerator DoLoopOperation(System.Func<float, bool> whileFunc, YieldInstruction interval, System.Action operation)
	{
		float startTime = Time.unscaledTime;
		while (whileFunc(Time.unscaledTime - startTime))
		{
			operation();
			yield return interval;
		}
	}

	#endregion

	#region wait

	public static Coroutine WaitOperation(System.Func<bool> waitUntil, System.Action operation, MonoBehaviour behaviour = null)
	{
		return StartCoroutine(DoWaitOperation(waitUntil, operation), behaviour);
	}

	private static IEnumerator DoWaitOperation(System.Func<bool> waitUntil, System.Action operation = null)
	{
		if (operation != null)
		{
			if (waitUntil != null)
			{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				while (!waitUntil())
				{
					yield return null;
				}
#else
				yield return new WaitUntil(waitUntil);
#endif
			}
			operation();
		}
	}

	#endregion

	#region lag

	public static Coroutine EndOfLagOperation(System.Action operation, MonoBehaviour behaviour = null)
	{
		return StartCoroutine(DoEndOfLagOperation(operation, behaviour));
	}

	private static IEnumerator DoEndOfLagOperation(System.Action operation, MonoBehaviour behaviour = null)
	{
		if (operation != null)
		{
			yield return WaitForEndOfLag(behaviour);
			operation();
		}
	}

	public static Coroutine WaitForEndOfLag(MonoBehaviour behaviour = null)
	{
		return StartCoroutine(DoWaitForEndOfLag(), behaviour);
	}

	private const int LAG_FRAME_COUNT_MAX = 20;
	private const int LAG_CHECK_FRAME_COUNT = 3;
	private const float LAG_CHECK_THRESHOLD = 0.001F;

	private static IEnumerator DoWaitForEndOfLag()
	{
		float maxFrame = Time.frameCount + LAG_FRAME_COUNT_MAX;

		List<float> deltaTimeList = new List<float>();
		deltaTimeList.Add(Time.unscaledDeltaTime);
		yield return null;
		deltaTimeList.Add(Time.unscaledDeltaTime);
		while (Time.frameCount < maxFrame)
		{
			yield return null;
			deltaTimeList.Add(Time.unscaledDeltaTime);
			if (deltaTimeList.Count > LAG_CHECK_FRAME_COUNT)
			{
				deltaTimeList.RemoveAt(0);
			}
			float variance = GetVariance(deltaTimeList);
			if (variance < LAG_CHECK_THRESHOLD)
			{
				yield return null;
				break;
			}
		}
		deltaTimeList.Clear();
	}

	private static float GetVariance(ICollection<float> nums)
	{
		int count = nums.Count;
		float average = System.Linq.Enumerable.Average(nums);
		float varianceSum = 0;
		foreach (float num in nums)
		{
			varianceSum += Mathf.Pow(num - average, 2);
		}
		return varianceSum / count;
	}

	#endregion
}
