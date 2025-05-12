using System;
using UnityEngine;

public class State<T> {
    public virtual void Enter(StateMachine<T> obj) { }
    public virtual void Exit(StateMachine<T> obj) { }
    public virtual void Update(StateMachine<T> obj) { }

    public virtual void BDown(StateMachine<T> obj) {

    }

    public virtual void FixedUpdate(StateMachine<T> obj) {

    }

    public virtual void OnCollisionEnter2D(Collision2D col, StateMachine<T> obj) {

    }

    public virtual string GetDebugName() {
        return "";
    }

    public virtual void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<T> obj) {

    }

    public virtual void CursorEnterSpecialZone(StateMachine<T> obj) {

    }


    public virtual void CursorExitxdSpecialZone(StateMachine<T> obj) {

    }

    public virtual bool isGamePlayState() {
        return false;
    }
}
