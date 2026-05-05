using UnityEngine;

public class LlamaradaArcangel : PoderBase
{
    [SerializeField] private float dmgBase = 30f;
    [SerializeField] private float escaladoDmgAura = 0.45f;
    [SerializeField] private float escaladoDmgNivel = 5f;
    [SerializeField] private float dmgPorcBase = 1f;
    [SerializeField] private float dmgPorcNivel = 0.25f;
    [SerializeField] private float dmgPorcAura = 0.01f;
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        float dmg = GetDmg();
        float dmgPorcentual = this.Handler.ModDmg * (dmgPorcBase + dmgPorcAura * this.Handler.Aura + dmgPorcNivel * this.Nivel);

        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
        {
            return;
        }
        HandlerEstados estados = other.GetComponent<HandlerEstados>();

        if (vida.isActiveAndEnabled)
        {
            vida.RecibirDmg(dmg, Handler);
            vida.RecibirDmgPorcentualProlongado(dmgPorcentual, Handler, 3f);
        }
    }

    protected override float CalcularDmgBase()
    {
        float dmg = dmgBase + escaladoDmgAura * this.Handler.Aura + escaladoDmgNivel * this.Nivel;
        return dmg;
    }
    public override string GetNombre() { return "Llama del Arcangel"; }
    public override string GetDescripcion() { return "Lanza un proyectil de avance lento pero letal, que atraviesa a los enemigos y los envuelve en llamas, infligiendo daño continuo."; }
}
