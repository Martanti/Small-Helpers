//#define CoroutineTest

using Godot;
using System.Collections;
using System;

public class WaitSeconds
{
	public WaitSeconds(double secondsToWait)
	{
		Seconds = secondsToWait;
	}
	public double Seconds;
}

public partial class CoroutineManager : Godot.Node
{
	private class CoroutineData
	{
		public object Owner { get; set; }
		public IEnumerator Coroutine { get; set; }
		public WaitSeconds SecondsToWait { get; set; }
	};

	// Array size 
	private const int ArraySize = 1024;
	private static CoroutineData[] _coroutines;
	private static int _nextCoroutineIndex;

	public override void _Ready()
	{
		_coroutines = new CoroutineData[ArraySize];
		_nextCoroutineIndex = 0;

#if CoroutineTest
		TestCoroutineHandle = DoMyCoroutine();
		CoroutineManager.StartCoroutine(this, TestCoroutineHandle);
#endif
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < _nextCoroutineIndex; i++)
		{
			var coroutineData = _coroutines[i];
			if (coroutineData.SecondsToWait.Seconds > 0f)
			{
				coroutineData.SecondsToWait.Seconds -= delta;
				if (coroutineData.SecondsToWait.Seconds > 0f)
				{
					continue;
				}
			}

			if (coroutineData.Owner == null || !coroutineData.Coroutine.MoveNext())
			{
				// The owner is no longer valid or the coroutine has ended
				// therefore scrap the 
				// coroutine by no longer iterating over it
				// and overwriting it on the next add and just continue
				// as per usual.
				ScrapCoroutineAt(i);
				coroutineData = _coroutines[i];
			}

			HandleCoroutineReturnResult(ref coroutineData);
		}
	}

	public static void StartCoroutine(object owner, IEnumerator coroutine)
	{
		if (_nextCoroutineIndex > _coroutines.Length)
		{
			Array.Resize(ref _coroutines, _coroutines.Length + ArraySize);
		}

		var newCoroutineData = new CoroutineData()
		{
			Owner = owner,
			Coroutine = coroutine,
			SecondsToWait = new WaitSeconds(0)
		};

		HandleCoroutineReturnResult(ref newCoroutineData);
		_coroutines[_nextCoroutineIndex++] = newCoroutineData;
	}

	public static void StopAllCoroutines()
	{
		_nextCoroutineIndex = 0;
	}

	public static void TerminateCoroutine(IEnumerator coroutine)
	{
		for (int i = 0; i < _nextCoroutineIndex; i++)
		{
			if (_coroutines[i].Coroutine.Equals(coroutine))
			{
				ScrapCoroutineAt(i);
				break;
			}
		}
	}

	private static void ScrapCoroutineAt(int index)
	{
		--_nextCoroutineIndex;
		(_coroutines[index], _coroutines[_nextCoroutineIndex]) =
			(_coroutines[_nextCoroutineIndex], _coroutines[index]);
	}

	private static void HandleCoroutineReturnResult(ref CoroutineData coroutineData)
	{
		switch (coroutineData.Coroutine.Current)
		{
			case WaitSeconds secondsToWait:
				coroutineData.SecondsToWait = secondsToWait;
				break;
			default:
				break;
		}
	}

#if CoroutineTest
	private IEnumerator TestCoroutineHandle;
	private IEnumerator DoMyCoroutine()
	{
		GD.Print("First print");
		yield return null;
		GD.Print("Wait 4 seconds");
		yield return new WaitSeconds(4.0);
		GD.Print("4 seconds passed!");

		CoroutineManager.StartCoroutine(this, SecondCoroutine());

		while (true)
		{
			yield return new WaitSeconds(1.0);
			GD.Print("ping");
		}
	}

	private IEnumerator SecondCoroutine()
	{
		GD.Print("Will kill the first coroutine after 5 seconds");
		yield return new WaitSeconds(5.0);
		CoroutineManager.TerminateCoroutine(TestCoroutineHandle);
		GD.Print("Goodbye!");
	}
#endif

}
