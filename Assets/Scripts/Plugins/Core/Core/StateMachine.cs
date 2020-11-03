using System;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;


[Serializable]
public class StateMachine<TLabel>
{
	private static void Empty() { }
	private static bool AlwaysPerformTransition(TLabel label) { return true; }

	[SerializeField]
	private TLabel		m_CurrentStateLabel;
	private State		m_CurrentState;

	private readonly Dictionary<TLabel, State> stateDictionary;

	/// <summary>Returns the label of the current state. </summary>
	public TLabel CurrentState
	{
		get => m_CurrentStateLabel;
		
		set => _ChangeState(value);
	}

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	private class State
	{
		public readonly Action onStart;
		public readonly Action onStop;
		public readonly Action onUpdate;
		public readonly Action onReEnter;
		
		public readonly Func<TLabel, bool>	transitionFilter;

		public State(Action onStart, Action onUpdate, Action onStop, Action onReEnter, Func<TLabel, bool> transitionFilter)
		{
			this.onStart = onStart;
			this.onUpdate = onUpdate;
			this.onStop = onStop;
			this.onReEnter = onReEnter;
			this.transitionFilter = transitionFilter;
		}

	}

	public interface IStateObject
	{
		StateMachine<TLabel>	StateMachine { set; }

		void iOnStart();
		void iOnStop();
		void iOnUpdate();
		void iOnReEnter();
		bool iTransitionFilter(TLabel label);
	}
	
	public abstract class StateObject<TState> : IStateObject
	{
		public abstract TState	State { get; }
		
		public StateMachine<TLabel> StateMachine { get; set; }

		public virtual void iOnStart() { }
		public virtual void iOnStop() { }
		public virtual void iOnUpdate() { }
		public virtual void iOnReEnter() { }
		public virtual bool iTransitionFilter(TLabel label) { return true; }

		protected bool Equals(StateObject<TState> other)
		{
			return State.Equals(other.State);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;

			if (obj is StateObject<TState> stateObj)
				return Equals(stateObj);

			if (obj is TState state)
				return State.Equals(state);

			return false;
		}

		public override int GetHashCode()
		{
			return State.GetHashCode();
		}
	}

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Constructs a new StateMachine.
	/// </summary>
	public StateMachine()
	{
		stateDictionary = new Dictionary<TLabel, State>();
	}

	/// <summary>
	/// This method should be called every frame.
	/// </summary>
	public void Update()
	{
		m_CurrentState?.onUpdate?.Invoke();
	}

    /// <summary>
    /// Adds a state, and the delegates that should run 
    /// when the state starts, stops, 
    /// and when the state machine is updated.
    /// 
    /// Any delegate can be null, and wont be executed.
    /// </summary>
    /// <param name="label">The name of the state to add.</param>
    /// <param name="onStart">The action performed when the state is entered.</param>
    /// <param name="onUpdate">The action performed when the state machine is updated in the given state.</param>
    /// <param name="onStop">The action performed when the state machine is left.</param>
    /// <param name="onReEnter">The action performed when the state machine calls self.</param>
    /// <param name="transitionFilter">Allow transition to the state function.</param>
    public void AddState(TLabel label, Action onStart = null, Action onUpdate = null, Action onStop = null, Action onReEnter = null, Func<TLabel, bool> transitionFilter = null)
	{
        // add or override state
		stateDictionary[label] = new State(onStart, onUpdate, onStop, onReEnter ?? Empty, transitionFilter ?? AlwaysPerformTransition);
	}

	public void AddState<T>(T label) where T : IStateObject, TLabel
	{
        // override state
        stateDictionary.Remove(label);

		label.StateMachine = this;
        stateDictionary.Add(label, new State(label.iOnStart, label.iOnUpdate, label.iOnStop, label.iOnReEnter, label.iTransitionFilter));
	}
	
	/// <summary>
	/// Adds a sub state machine for the given state.
	///
	/// The sub state machine need not be updated, as long as this state machine
	/// is being updated.
	/// </summary>
	/// <typeparam name="TSubStateLabel">The type of the sub-machine.</typeparam>
	/// <param name="label">The name of the state to add.</param>
	/// <param name="subMachine">The sub-machine that will run during the given state.</param>
	/// <param name="subMachineStartState">The starting state of the sub-machine.</param>
	public void AddState<TSubStateLabel>(TLabel label, StateMachine<TSubStateLabel> subMachine, TSubStateLabel subMachineStartState)
	{
		AddState(label, () => subMachine._ChangeState(subMachineStartState), subMachine.Update);
	}

	public void SetState<T>(T key)
	{
		// execution for states with custom equality comparer
		foreach (var n in stateDictionary)
		{
			if (n.Key.Equals(key))
			{
				_ChangeState(n.Key);
				return;
			}
		}
		
#if UNITY_EDITOR
		throw new ArgumentOutOfRangeException($"State \"{key.ToString()}\" not presented in dictionary.");
#endif
	}

    public void SetState<TKey, TState>(TKey key, Action<TState> stateActivation) where TState : TLabel
    {
        // execution for states with custom equality comparer
        foreach (var n in stateDictionary)
        {
            if (n.Key.Equals(key))
            {
                stateActivation.Invoke((TState)n.Key);
                _ChangeState(n.Key);
                return;
            }
        }
		
#if UNITY_EDITOR
        throw new ArgumentOutOfRangeException($"State \"{key.ToString()}\" not presented in dictionary.");
#endif
    }

    public TLabel GetState<TKey>(TKey key)
    {
        foreach (var n in stateDictionary)
            if (n.Key.Equals(key))
                return n.Key;

        return default;
    }

    public TType GetState<TType>()
    {
        foreach (var n in stateDictionary)
            if (n.Key is TType state)
                return state;

        return default;
    }

	public void SetState(TLabel key)
	{
		_ChangeState(key);
	}

    public bool TryGetCurrentState<TState>(out TState state)
    {
        if (m_CurrentState is TState result)
        {
            state = result;
            return true;
        }

        state = default;
        return false;
    }

	/// <summary>
	/// Discard current state
	/// </summary>
	public void Stop()
	{
		m_CurrentState?.onStop();
		m_CurrentState = null;

		m_CurrentStateLabel = default;
	}

	/// <summary>
	/// Returns the current state name
	/// </summary>
	public override string ToString()
	{
		return CurrentState.ToString();
	}

	/// <summary>
	/// Changes the state from the existing one to the state with the given label.
	/// </summary>
	private void _ChangeState(TLabel stateLable)
	{
#if UNITY_EDITOR
		if (stateDictionary.ContainsKey(stateLable) == false)
			throw new ArgumentOutOfRangeException($"State \"{stateLable.ToString()}\" not presented in dictionary.");
#endif

		var dicState = stateDictionary[stateLable];

		if (m_CurrentState != null)
		{
			// if transition allowed
			if (m_CurrentState.transitionFilter(stateLable))
			{
				if (stateLable.Equals(m_CurrentStateLabel))
				{
					// same state, activate ReEnter only if not null, else normal behaviour
					if (m_CurrentState.onReEnter != null)
					{
						// activate ReEnter
						m_CurrentState.onReEnter.Invoke();
					}
					else
					{
						// reactivate current state
						m_CurrentState.onStop?.Invoke();
						m_CurrentState.onStart?.Invoke();
					}
				}
				else
				{
					// switch state
					m_CurrentState.onStop?.Invoke();
					m_CurrentState = dicState;
                    m_CurrentStateLabel = stateLable;
					m_CurrentState.onStart?.Invoke();
				}
			}
		}
		else
		{	// set new state
			m_CurrentState = dicState;
            m_CurrentStateLabel = stateLable;
			m_CurrentState.onStart?.Invoke();
		}
    }

}