

public class StateMachine<T>  {
    public State<T> currentState;
    public T target;
    public StateMachine(State<T> startState, T obj) {
        currentState = startState;
        target = obj;
        currentState.Enter(this);
    }
    public void ChangeState(State<T> state) {
        State<T> oldState = currentState;
        currentState = state;
        oldState.Exit(this);
        state.Enter(this);
    }
    public void Update() {

        currentState.Update(this);
    }

    public void FixedUpdate() {
        currentState.FixedUpdate(this);
    }
}
