using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class RangedSM : BaseEnemySM
{
    public override FollowEnemy Follow { get; protected set; }
    public HuidaRango Huida;
    public PoderRango Poder;


    [SerializeField] protected float rangoHuida = 10f;    // si el jugador está MUY cerca, huir
    [SerializeField] protected float rangoPoder = 14f;
    public float RangoHuida => rangoHuida;
    public float RangoPoder => rangoPoder;
    public List<PoderBase> poderes;
    protected override void Awake()
    {
        base.Awake();
        foreach (PoderBase prefab in poderes )
        {
            PoderBase poder = Instantiate(prefab, transform);
            poder.gameObject.SetActive(true);
            poder.SetHandler(HandlerPoder);
            HandlerPoder.AgregarPoder(poder);
        }
        Follow = new FollowRango(this);
        Poder = new PoderRango(this);
        Huida = new HuidaRango(this);
    }

    protected override BaseState GetInitialState() => Wander;

}
