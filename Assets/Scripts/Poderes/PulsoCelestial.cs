using UnityEngine;

public class PulsoCelestial : PoderBase
{
    [SerializeField] private float dmgBase = 10f;
    [SerializeField] private float escaladoDmgAura = 1f;
    [SerializeField] private float escaladoDmgNivel = 5f;
    [SerializeField] private float tiempoRalentizacion = 1.5f;
    [SerializeField] private float tiempoRalentizacionNivel = 0.25f;
    [SerializeField] private float ralentizacionBase = 20f;
    [SerializeField] private float escaladoRalentizacionAura = 0.20f; 
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        float dmg = GetDmg();

        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
        {
            return;
        }
        HandlerEstados estados = other.GetComponent<HandlerEstados>();
        if (estados != null) {  estados.AplicarRalentizado(tiempoRalentizacion + tiempoRalentizacionNivel * this.Nivel, ralentizacionBase + this.handler.Aura * escaladoRalentizacionAura); }
        vida.RecibirDmg(dmg, Handler);
        Destroy(cuerpo.gameObject);
    }
    protected override float CalcularDmgBase()
    {
        float dmg = this.Handler.ModDmg * (dmgBase + escaladoDmgAura * this.Handler.Aura + escaladoDmgNivel * this.Nivel);
        return dmg;
    }
    public override float GetDmg()
    {
        return this.Handler.ModDmg * CalcularDmgBase();
    }
    public override string GetNombre() { return "Pulso Celestial"; }
    public override string GetDescripcion() { return "Dispara un pulso de energía a gran velocidad que, al impactar con el primer enemigo, reduce drásticamente su velocidad de movimiento."; }
}