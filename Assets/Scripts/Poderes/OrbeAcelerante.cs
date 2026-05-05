using UnityEngine;

public class OrbeAcelerante : PoderBase
{
    [SerializeField] float dmgBase = 15f;
    [SerializeField] float escaladoNivel = 2.5f;
    [SerializeField] float escaladoAura = 0.3f;
    [SerializeField] float tiempoAdelanto = 1f;
    [SerializeField] float tiempoAdelantoNivel = 0.25f;
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        HandlerPoder h = this.Handler;
        foreach(PoderBase poder in h.Poderes)
        {
            poder.Adelantar(tiempoAdelanto + tiempoAdelantoNivel * this.Nivel);
        }
        float dmg = GetDmg();
        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
        {
            return;
        }
        vida.RecibirDmg(dmg, Handler);
        Destroy(cuerpo.gameObject);
    }
    protected override float CalcularDmgBase()
    {
        float dmg = dmgBase + escaladoAura * this.Handler.Aura + escaladoNivel * this.Nivel;
        return dmg;
    }
    public override string GetDescripcion() { return "Lanza un orbe a gran velocidad que, al impactar, reduce el tiempo de recarga de tus demás poderes."; }
    public override string GetNombre() { return "Orbe Acelerante"; }
}

