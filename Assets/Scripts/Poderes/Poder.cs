using UnityEngine;

public class Bola : PoderBase
{
    [SerializeField] private float dmgBase = 3f;
    [SerializeField] private float escalado = 0.1f;
    [SerializeField] private float escaladoNivel = 5f;
    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        punto.y = punto.y + 0.2f;
        base.Lanzar(origen, punto, capaObjetivo);
    }
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        float dmg = GetDmg();        
        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        HandlerEstados estados = other.GetComponentInChildren<HandlerEstados>();
        if (vida == null)
        {
            return;
        }

        HandlerVida caster = handler.GetComponent<HandlerVida>();
        HandlerEstados casterEstado = handler.GetComponentInParent<HandlerEstados>();
        if (vida == null)
        {
            return;
        }
        vida.RecibirDmgPorcentual(dmg, this.Handler);
        casterEstado.AplicarRalentizado(10f,50f);
        casterEstado.AplicarAcelerado(5f, 150f);
        caster.RecibirCuraPorcentual(dmg);
    }
    protected override float CalcularDmgBase()
    {
        float dmg = dmgBase + escalado * this.Handler.Aura + escaladoNivel * this.Nivel;
        return dmg;
    }
    public override string GetDescripcion() { return ""; }
    public override string GetNombre() { return ""; }
}
