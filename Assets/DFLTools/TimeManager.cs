using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Updates and manages Timers
/// </summary>
public class TimeManager : MonoBehaviour
{
	#region Private Fields

	LinkedList<Timer> _adding;
	LinkedList<Timer> _removing;
	LinkedList<Timer> _timers;

	#endregion

	#region Public Properties

	/// <summary>
	/// Attempts to retrieve an instance of the TimeManager component 
	/// </summary>
	public static TimeManager Find
	{
		get { return GameObject.FindObjectOfType<TimeManager>(); }
	}

	#endregion

	#region Private Properties

	LinkedList<Timer> Adding
	{
		get
		{
			if (_adding == null)
			{
				_adding = new LinkedList<Timer>();
			}
			return _adding;
		}
	}

	LinkedList<Timer> Removing
	{
		get
		{
			if (_removing == null)
			{
				_removing = new LinkedList<Timer>();
			}
			return _removing;
		}
	}

	#endregion

	#region Public Methods

	public void AddTimer(Timer t)
	{
		Adding.AddLast(t);
	}

	/// <summary>
	/// Creates a new timer. 
	/// </summary>
	/// <param name="onComplete"> Method called on completion of timer. </param>
	/// <param name="onTick">    
	/// Method called on update tick. 
	/// <para/>
	/// Delta time is passed as a parameter. 
	/// </param>
	/// <param name="onTime">    
	/// A Dictionary that maps methods to points in time. 
	/// <para/>
	/// Each method is called as the timer passes the associated time. 
	/// </param>
	/// <returns></returns>
	public Timer GetNewTimer
		(float span = 1,
		Action onComplete = null,
		Action<float> onTick = null,
		Dictionary<float, Action> onTime = null,
		bool runWhilePaused = false,
		bool loops = false)
	{
		var result = new Timer(span, onComplete, onTick, onTime, runWhilePaused, loops);
		Adding.AddLast(result);
		return result;
	}

	public void RemoveTimer(Timer t)
	{
		Removing.AddLast(t);
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		_timers = _timers ?? new LinkedList<Timer>();
		_adding = _adding ?? new LinkedList<Timer>();
		_removing = _removing ?? new LinkedList<Timer>();
	}

	void Update()
	{
		foreach (var t in _adding)
		{
			if (t != null && !_timers.Contains(t))
				_timers.AddLast(t);
		}
		_adding.Clear();

		foreach (var t in _timers)
			t.Advance(Time.deltaTime);

		foreach (var t in _removing)
			_timers.Remove(t);
		_removing.Clear();
	}

	#endregion
}

[Serializable]
public class Timer
{
	#region Private Fields

	[SerializeField]
	public string id;

	[SerializeField]
	float _current;

	[SerializeField]
	float _end;

	[SerializeField]
	bool _loop;

	TimeManager _manager;

	Dictionary<float, Action> _onTime;

	[SerializeField]
	bool _running;

	[SerializeField]
	bool _runWhilePaused;

	#endregion

	#region Public Properties

	/// <summary>
	/// Instantiates a deep copy of the Timer.
	/// <para/>
	/// The new timer will be created in a paused state.
	/// </summary>
	public Timer Clone
	{
		get
		{
			var result = new Timer();
			{
				result.End = End;
				result.OnComplete = OnComplete;
				result.OnTick = OnTick;
				result.Loop = Loop;
				result.RunWhilePaused = RunWhilePaused;
			}

			//Perform deep copy of dictionary
			foreach(var p in OnTime)
			{
				result.OnTime[p.Key] = new Action(p.Value);
			}

			result.Current = Current;
			result.Running = false;

			return result;
		}
	}

	public bool Complete { get { return Current >= End; } }

	public float Completion { get { return Mathf.Clamp01(Current / End); } }

	public float Current { get { return _current; } set { _current = value; } }

	public float End { get { return _end; } set { _end = value; } }

	public bool Loop { get { return _loop; } set { _loop = value; } }

	public Action OnComplete { get; set; }

	public Action<float> OnTick { get; set; }

	public Dictionary<float, Action> OnTime
	{
		get
		{
			if (_onTime == null)
			{
				_onTime = new Dictionary<float, Action>();
			}
			return _onTime;
		}
		private set
		{
			_onTime = value;
		}
	}

	public float Remaining { get { return End - Current; } }

	public float Remaining01 { get { return 1f - Completion; } }

	public bool Running { get { return _running; } private set { _running = value; } }

	public bool RunWhilePaused { get { return _runWhilePaused; } set { _runWhilePaused = value; } }

	#endregion

	#region Private Properties

	TimeManager Manager
	{
		get
		{
			if (!_manager)
			{
				_manager = TimeManager.Find;
			}
			return _manager;
		}
		set { _manager = value; }
	}

	#endregion

	#region Public Constructors



	public Timer
		(float span = 1,
		Action onComplete = null,
		Action<float> onTick = null,
		Dictionary<float, Action> onTime = null,
		bool runWhilePaused = false,
		bool loops = false)
	{
		End = span;
		OnComplete = onComplete;
		OnTick = onTick;
		RunWhilePaused = runWhilePaused;
		Loop = loops;
		OnTime = onTime;
	}

	#endregion

	#region Public Methods

	public void Advance(float deltaTime = -1)
	{
		if (deltaTime <= -1)
		{
			deltaTime = Time.deltaTime;
		}

		//todo: establish pause state
		//if (!GameState.Paused || RunWhilePaused)
		{
			if (Running && !Complete)
			{
				Current += deltaTime;

				if (OnTick != null)
					OnTick(deltaTime);

				foreach (var t in OnTime.Keys)
				{
					if (Current > t && Current - deltaTime <= t)
					{
						if (OnTime[t] != null)
							OnTime[t]();
					}
				}

				if (Complete)
				{
					if (OnComplete != null)
						OnComplete();
					if (Loop)
						Current -= End;
					else
						Pause();
				}
			}
		}
	}

	/// <summary>
	/// Format string. Supplies values for current, end, and remaining times.
	/// <para/>
	/// Seconds: {0,1,2} Minutes: {3,4,5} Hours: {6,7,8}
	/// <para/>
	/// Example: "{3}:{0:00}/{4}:{1:00}" produces a string representation of the time in the
	/// format "C:cc/E:ee", where C:cc is the current time in minutes and seconds, and E:ee is 
	/// the end time
	/// </summary>
	/// <param name="format"></param>
	/// <returns></returns>
	public string F(string format)
	{
		var c = TimeSpan.FromSeconds(Current);
		var e = TimeSpan.FromSeconds(End);
		var r = TimeSpan.FromSeconds(Remaining);
		return String.Format(format,
			c.Seconds, e.Seconds, r.Seconds,
			c.Minutes, e.Minutes, r.Minutes,
			c.Hours, e.Hours, r.Hours);
	}

	/// <summary>
	/// Pause the timer. 
	/// <para/>
	/// Tries to leave a TimeManager if possible.
	/// </summary>
	public void Pause()
	{
		Running = false;

		if (Manager)
			Manager.RemoveTimer(this);
	}

	/// <summary>
	/// Resume the timer. 
	/// <para/>
	/// Tries to join a TimeManager if possible 
	/// </summary>
	public void Resume()
	{
		Running = true;

		if (Manager)
			Manager.AddTimer(this);
	}

	/// <summary>
	/// Run the timer from the beginning. 
	/// <para/>
	/// Tries to join a TimeManager if possible.
	/// </summary>
	public void Run()
	{
		Current = 0;
		Running = true;

		if (Manager)
			Manager.AddTimer(this);
	}

	/// <summary>
	/// Stop and reset the timer. 
	/// <para/>
	/// Tries to leave a TimeManager if possible.
	/// </summary>
	public void Stop()
	{
		Current = 0;
		Running = false;

		if (Manager)
			Manager.RemoveTimer(this);
	}

	#endregion
}