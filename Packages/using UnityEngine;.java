using UnityEngine;


public class StateMachine<T> {
    public State<T> currentState;
    public T target;
    public StateMachine(State<T> startState, T obj) {
        currentState = startState;
        target = obj;
        currentState.Enter(this);
    }
    public void ChangeState(State<T> state) {
        currentState.Exit(this);
        currentState = state;
        state.Enter(this);
    }
    public void Update() {

        currentState.Update(this);
    }

    public void FixedUpdate() {
        currentState.FixedUpdate(this);
    }
}

public class State<T> {
    public virtual void Enter(StateMachine<T> obj) { }
    public virtual void Exit(StateMachine<T> obj) { }
    public virtual void Update(StateMachine<T> obj) { }
    public virtual void BDown(StateMachine<T> obj) {

    }
    public virtual void FixedUpdate(StateMachine<T> obj) {

    }
    public virtual string GetDebugName() {
        return "";
    }
}


public class PlayerController : MonoBehaviour {

    public Rigidbody rb;
    public Animator anim;

    public StateMachine<PlayerController> machine;

    private void Awake() {
        machine = new StateMachine<PlayerController>(new WalkState(), this);
    }

    private void Update() {
        machine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "cliff") {
            machine.ChangeState(new JumpDownCliffState(transform.position + new Vector3(0, -1, 0)));
        }
    }

}



public class WalkState : State<PlayerController> {

    public override void Update(StateMachine<PlayerController> obj) {
        obj.target.rb.velocity = new Vector3();
    }

}


public class JumpDownCliffState : State<PlayerController> {

    public Vector3 targetPos;

    public JumpDownCliffState(Vector3 pos) {
        targetPos = pos;
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (obj.target.transform.position != targetPos) {
            obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, targetPos, 5 * Time.deltaTime);
        } else {
            obj.ChangeState(new WalkState());
        }
    }
}