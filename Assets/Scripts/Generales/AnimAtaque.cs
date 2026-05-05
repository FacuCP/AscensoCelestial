using Unity.VisualScripting;
using UnityEngine;

public class AnimAtaque : StateMachineBehaviour
{
    private bool faseUno = false, faseDos = false;
    private bool finLlamado = false;
    private float tiempoFaseUno = 0.4f, tiempoFaseDos = 0.8f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        faseUno = false;
        faseDos = false; // reset al entrar en el estado
        finLlamado = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandlerAnimacion handler = animator.GetComponent<HandlerAnimacion>();
        if (handler == null) return;
        if (!faseUno && stateInfo.normalizedTime >= tiempoFaseUno)
        {
            faseUno = true;
            handler.AtaqueFaseUno();
            
        }
        if (!faseDos && stateInfo.normalizedTime >= tiempoFaseDos)
        {
            faseDos= true;
            handler.AtaqueFaseDos();
        }
        if ( stateInfo.normalizedTime >= 1f)
        {
            finLlamado = true;
            handler.FinAtaque();
        }
    }

}