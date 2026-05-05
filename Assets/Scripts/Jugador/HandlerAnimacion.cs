using System;
using System.Collections.Generic;
using UnityEngine;
public class HandlerAnimacion : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator Animator =>animator;

    [Header("Idle")]
    public AnimationClip idleFront;
    public AnimationClip idleBack;
    public AnimationClip idleSide;

    [Header("Walk")]
    public AnimationClip walkFront;
    public AnimationClip walkBack;
    public AnimationClip walkSide;

    [Header("Attack")]
    public AnimationClip attack;

    private void Awake()
    {
        Animator anim = GetComponent<Animator>();

        AnimatorOverrideController overrideController =
            new AnimatorOverrideController(anim.runtimeAnimatorController);

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
            new List<KeyValuePair<AnimationClip, AnimationClip>>();

        overrideController.GetOverrides(overrides);

        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            string nombre = transform.parent.name;

            // IDLE
            if (original.name.Contains("IddleFront"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, idleFront);
            else if (original.name.Contains("IddleBack"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, idleBack);
            else if (original.name.Contains("IddleSide"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, idleSide);

            // WALK
            else if (original.name.Contains("WalkFront"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, walkFront);
            else if (original.name.Contains("WalkBack"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, walkBack);
            else if (original.name.Contains("WalkSide"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, walkSide);

            // ATTACK
            else if (original.name.Contains("Attack"))
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, attack);
        }

        overrideController.ApplyOverrides(overrides);
        anim.runtimeAnimatorController = overrideController;
    }
    public void SetMovimiento(Vector3 direccion) {
        if (direccion != Vector3.zero)
        {
            animator.SetFloat("movV", direccion.z);
            animator.SetFloat("movH", Mathf.Abs(direccion.x)); // solo magnitud
            GirarHacia(direccion); // gira el personaje hacia la dirección de movimiento
        }
        animator.SetBool("isMoving", true);
    }

    public void Frenar() { animator.SetBool("isMoving", false); }
    public void Atacar(float rafaga, Vector3 direccion)
    {
        if (animator.GetBool("ataque")) return;
        animator.SetFloat("movV", direccion.z);
        animator.SetFloat("movH", Mathf.Abs(direccion.x));

        GirarHacia(direccion); // gira el personaje hacia la dirección del ataque

        animator.SetFloat("velAtq", rafaga);
        animator.SetBool("ataque", true);
    }

    // Función centralizada para girar el personaje según una dirección
    private void GirarHacia(Vector3 direccion)
    {
        if (direccion.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = direccion.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
    public event Action AparecerCollider;
    public event Action DesaparecerCollider;
    public event Action FinalAtaque;
    public event Action AtaqueReset;

    public void AtaqueFaseUno() { AparecerCollider?.Invoke(); }
    public void AtaqueFaseDos() { DesaparecerCollider?.Invoke(); }
    public void FinAtaque()
    {
        animator.SetBool("ataque", false);
        FinalAtaque?.Invoke();
        AtaqueReset?.Invoke();
    }
}