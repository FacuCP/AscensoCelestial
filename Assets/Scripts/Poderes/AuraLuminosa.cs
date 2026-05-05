using UnityEngine;

public class AuraLuminosa : PoderBase
{
    [SerializeField] private float dmgBase = 5f;
    [SerializeField] private float escaladoNivel = 2.5f;
    [SerializeField] private float escaladoAura = 0.1f;
    [SerializeField] private float consBase = 30;
    [SerializeField] private float consAura = 0.3f;
    [SerializeField] private float consNivel = 5;


    private bool consumiendo = true;

    [SerializeField] private float auraMax = 500f;
    [SerializeField] private float escalaMin = 1f;
    [SerializeField] private float escalaMax = 2.5f;

    [SerializeField] private float velocidadReduccion = 5f;
    [SerializeField] private float tiempoFade = 2.5f; public float AuraMax => auraMax;
    public float EscalaMin => escalaMin;
    public float EscalaMax => escalaMax;
    public float VelocidadReduccion => velocidadReduccion;
    public float TiempoFade => tiempoFade;

    private float acumuladorDmg;

    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        capaObjetivo |= (1 << LayerMask.NameToLayer("Habilidad Jugador"));
        capaObjetivo |= (1 << LayerMask.NameToLayer("Habilidad Enemigo"));
        HandlerEstados estados = GetComponentInParent<HandlerEstados>();
        if (estados != null) { estados.AplicarParalizado(0.5f); }
        consumiendo = true;
        acumuladorDmg = 0;
        base.Lanzar(punto,origen, capaObjetivo, rotar);
    }

    public override string GetDescripcion()
    {
        return "Consume los poderes cercanos para liberar una explosión de aura que inflige daño en función de la energía absorbida.";
    }

    public override string GetNombre() { return "Aura Luminosa"; }
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        CuerpoPoder poder = other.GetComponent<CuerpoPoder>();
        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        if (consumiendo && poder)
        {   
            acumuladorDmg += poder.PoderPropietario.GetDmg();
            poder.Destruir();
        }
        else if (consumiendo == false && vida != null)
        {
            float dmg = GetDmg();
            vida.RecibirDmg(dmg, this.Handler);
        }
    }

    public void SetConsumiendo(bool valor) { 
        consumiendo = valor;   
    }
    protected override float CalcularDmgBase()
    {
        // Daño base normal (sin acumulador)
        float dmgBaseTotal = dmgBase + (escaladoNivel * Nivel) + (handler.Aura * escaladoAura);

        float porcentajeAcumulador = consBase / 100f; // base como porcentaje

        porcentajeAcumulador += (consNivel * Nivel) / 100f; // mejora por nivel

        porcentajeAcumulador += (consAura * handler.Aura) / 100f; // escalado por aura

        // Daño extra del acumulador
        float dmgAcumulador = acumuladorDmg * porcentajeAcumulador;

        // Total
        float dmgTotal = dmgBaseTotal + dmgAcumulador;

        return dmgTotal;
    }
}
