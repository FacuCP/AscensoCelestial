using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected BaseState currentState;

    protected virtual void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    protected void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }
    void FixedUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }
    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
