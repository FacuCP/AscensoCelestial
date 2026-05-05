using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MeleeSM : BaseEnemySM
{
    public override FollowEnemy Follow { get; protected set; }
    public AttackMelee Attack;

    [SerializeField] protected float rangoAtaqueJugador = 2f;   // atacar

    public float RangoAtaqueJugador => rangoAtaqueJugador;
    public List<ForjaBase> forjas;
    protected override void Awake()
    {
        base.Awake();
        Follow = new FollowMelee(this);
        Attack = new AttackMelee(this);

        foreach(ForjaBase f in forjas){
           ForjaBase forja = Instantiate(f, transform);
            forja.gameObject.SetActive(true);
            forja.SetHandler(HandlerAtaque);
            HandlerAtaque.AgregarForja(forja);
        }
    }

    protected override BaseState GetInitialState() => Wander;

}
