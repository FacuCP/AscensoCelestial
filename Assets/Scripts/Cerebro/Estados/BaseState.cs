using UnityEngine;

public class BaseState {
    protected StateMachine sm;

    public BaseState(StateMachine stateMachine)
    {
        this.sm = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }

    public virtual void OnAtaque() { }
    public virtual void OnPoder() { }
    public virtual void OnFavor() { }
    public virtual void OnCambio(float val) { }
    public virtual void OnMovimiento(Vector3 dir) { }

}
