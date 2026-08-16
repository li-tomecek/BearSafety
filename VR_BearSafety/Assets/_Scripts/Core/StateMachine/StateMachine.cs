using System;
using System.Collections.Generic;

public class StateMachine<T> where T : IState
{
    private readonly Dictionary<Type, T> _states = new();
    private bool _isTransitioning = false;

    public T CurrentState { get; private set; }
    public T PreviousState { get; private set; }


    public StateMachine(T initialState)
    {
        if (initialState == null)
            throw new ArgumentNullException(nameof(initialState));

        AddState(initialState);
        CurrentState = initialState;
        CurrentState.Enter();
    }


    public void AddState(T state)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        var stateType = state.GetType();

        if (_states.ContainsKey(stateType))
            throw new InvalidOperationException($"State {stateType.Name} already exists in state machine");

        _states[stateType] = state;
    }

    public void TransitionTo<TState>() where TState : T
    {
        if (_isTransitioning)
            throw new InvalidOperationException("Cannot transition during another transition");

        var stateType = typeof(TState);

        if (!_states.TryGetValue(stateType, out var nextState))
            throw new InvalidOperationException($"State {stateType.Name} not found in state machine");

        if (ReferenceEquals(CurrentState, nextState))
            return;

        _isTransitioning = true;

        CurrentState?.Exit();
        PreviousState = CurrentState;

        CurrentState = nextState;
        CurrentState.Enter();

        _isTransitioning = false;

        UnityEngine.Debug.Log(CurrentState.ToString());
    }

    public void ReturnToPreviousState()
    {
        if (PreviousState == null)
            return;

        if (_isTransitioning)
            return;

        _isTransitioning = true;

        CurrentState?.Exit();

        var stateToReturnTo = PreviousState;

        PreviousState = CurrentState;
        CurrentState = stateToReturnTo;

        CurrentState.Enter();

        _isTransitioning = false;

        UnityEngine.Debug.Log(CurrentState.ToString());
    }

    public bool TryGetState<TState>(out TState state) where TState : T
    {
        if (_states.TryGetValue(typeof(TState), out var validState))
        {
            state = (TState)validState;
            return true;
        }

        state = default;
        return false;
    }

    public virtual void Update() => CurrentState?.Update();

    public virtual void FixedUpdate() => CurrentState?.FixedUpdate();
}