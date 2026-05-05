using System.Collections.Generic;
using UnityEngine;

public class EspacioSeguro : PoderBase
{
    [SerializeField] private float auraMax = 500f;
    [SerializeField] private float escalaMin = 1f;
    [SerializeField] private float escalaMax = 2.5f;

    [SerializeField] private float dmgBase = 1;
    [SerializeField] private float dmgNivel = 0.25f;
    [SerializeField] private float dmgAura = 0.01f;
    [SerializeField] private float bendBase = 15;
    [SerializeField] private float bendNivel = 2.5f;
    [SerializeField] private float bendAura = 0.02f;
    [SerializeField] private float acelBase = 15;
    [SerializeField] private float acelNivel = 2.5f;
    [SerializeField] private float acelAura = 0.02f;
    public float AuraMax => auraMax;
    public float EscalaMin => escalaMin;
    public float EscalaMax => escalaMax;

    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        punto.y = punto.y + 0.2f;
        base.Lanzar(origen, punto, capaObjetivo, false);
    }
    protected override float CalcularDmgBase()
    {
        return dmgBase + dmgNivel * this.Nivel + dmgAura * handler.Aura;
    }
    public override string GetDescripcion()
    {
        return "Crea un área en el punto seleccionado. Dentro de ella, el jugador recibe bendiciones que aumentan su velocidad y regeneración de vida, mientras que los enemigos son maldecidos, ralentizados y reciben daño continuo.";
    }
    public override string GetNombre() { return "Espacio Seguro"; }
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        if (other == null) return;

        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        HandlerEstados estados = other.GetComponent<HandlerEstados>();
        float valorBend = bendBase + bendNivel * this.Nivel + bendAura * handler.Aura;
        float valorAcel = acelBase + acelNivel * this.Nivel + acelAura * handler.Aura;
        if (vida == null) return;

        if (aliado)
        {
            vida.RecibirCura(GetDmg());
            if (estados != null) { 
                estados.AplicarBendito(0.25f, valorBend);
                estados.AplicarAcelerado(0.25f, valorAcel);
            }
        }
        else
        {
            vida.RecibirDmg(GetDmg(), Handler);
            if (estados != null)
            {
                estados.AplicarMaldito(0.25f, valorBend);
                estados.AplicarRalentizado(0.25f, valorAcel);
            }
        }
    }
}
