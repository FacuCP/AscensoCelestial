using UnityEngine;

public class LuzDivina : PoderBase
{
    private int cargas = 0;
    [SerializeField] int maxCargas = 5;
    [SerializeField] float dmgBase = 15f;
    [SerializeField] float escaladoNivel = 2.5f;
    [SerializeField] float escaladoAura = 0.3f;
    [SerializeField] float dmgMaxCargas = 40f;
    [SerializeField] float dmgMaxCargasNivel = 10f;
    [SerializeField] float dmgMaxCargasAura = 0.5f;
    [SerializeField] float tiempoCegado = 5f;
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        HandlerEstados estados = null; 
        float dmg = GetDmg();
        if (cargas < maxCargas)
        {
            cargas++;
        }
        else { 
            cargas = 0;
            estados = other.GetComponent<HandlerEstados>();
            if (estados != null) estados.AplicarCiego(tiempoCegado);
        }

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
        float dmg;
        if (cargas < maxCargas)
        {
            dmg = dmgBase + escaladoAura * this.Handler.Aura + escaladoNivel * this.Nivel;
        }
        else
        {
            dmg = dmgMaxCargas + dmgMaxCargasAura * this.Handler.Aura + dmgMaxCargasNivel * this.Nivel;
        }
        return dmg;
    }
    public override string GetNombre() { return "Luz Divina"; }
    public override string GetDescripcion() { return "Dispara un proyectil que genera cargas con cada impacto, incrementando progresivamente su daño. Al alcanzar el máximo, desata un golpe devastador que inflige gran daño y ceguera."; }
}
