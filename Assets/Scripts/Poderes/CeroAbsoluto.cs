using UnityEngine;

public class CeroAbsoluto : PoderBase
{
    [SerializeField] float dmgBase = 15f;
    [SerializeField] float escaladoNivel = 2.5f;
    [SerializeField] float escaladoAura = 0.3f;
    [SerializeField] float tiempoCongelado = 1f;
    [SerializeField] float tiempoCongeladoNivel = 0.25f;
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        HandlerEstados estados = other.GetComponent<HandlerEstados>();
        float dmg = GetDmg();
        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
        {
            return;
        }

        if (estados != null) {
            float tiempo = tiempoCongelado + tiempoCongeladoNivel * this.Nivel ;
            estados.AplicarParalizado(tiempo);
        };
        vida.RecibirDmg(dmg, this.Handler);
        Destroy(cuerpo.gameObject);
    }
    protected override float CalcularDmgBase()
    {
        float dmg = dmgBase + escaladoAura * this.Handler.Aura + escaladoNivel * this.Nivel;
        return dmg;
    }

    public override string GetDescripcion()
    {
        return "Dispara un proyectil de frío extremo que, al impactar al primer objetivo, lo congela y lo deja completamente inmovilizado.";
    }
    public override string GetNombre() { return "Cero Absoluto"; }
}
