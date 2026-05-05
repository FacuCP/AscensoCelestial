using UnityEngine;
using System;

public class Estadisticas:MonoBehaviour
{
    [SerializeField, Header("Atributos Básicos")] private float vidaBase;

    [SerializeField] private float velocidadBase;

    [SerializeField] private float alientoBase;
    [SerializeField] private int regenVida;
    [SerializeField] private int regenAliento;
    [SerializeField, Header("Atributos Poder")] private float auraBase;
    [SerializeField] private float prisaBase;
    [SerializeField] private float alivioBase;
    [SerializeField] private int limitePoderesBase;

    [SerializeField, Header("Atributos Fuerza")] private float fuerzaBase;
    [SerializeField] private float suerteBase;
    [SerializeField] private float rafagaBase;
    [SerializeField] private int limiteForjasBase;


    [SerializeField, Header("Atributos Ataque")] private int dmgBase = 20;
    [SerializeField, Range(0f, 1.5f)] private float escaladoFuerza = 0.5f;
    [SerializeField,Range(1.5f, 2.5f)] private float critMultiplier = 1.5f;
    public float Vida => vida;
    public float Aliento => aliento;

    public float VelocidadUI => velocidad;
    public float Velocidad => velocidad/10;
    public float Aura => aura;
    public float Prisa => prisa;
    public float Alivio => alivio;
    public int LimitePoderes => limitePoderes;
    public float Fuerza => fuerza;
    public float Suerte => suerte; 
    public float Rafaga => rafaga;
    public float RafagaBase => rafagaBase;
    public int LimiteForjas => limiteForjas;
    public int DmgBase => dmgBase;
    public float EscaladoFuerza => escaladoFuerza;
    public float CritMultiplier => critMultiplier;

    public int RegenVida => regenVida;
    public int RegenAliento => regenAliento;

    public event Action CambioEstadisticas;

    private float vida,velocidad, aliento, aura, prisa, alivio, fuerza, suerte, rafaga;
    int limitePoderes, limiteForjas;

    private void Awake()
    {
        Reiniciar();
    }

    public void Reiniciar()
    {
        vida = vidaBase;
        velocidad = velocidadBase;
        aliento = alientoBase;
        aura = auraBase;
        prisa = prisaBase;
        alivio = alivioBase;
        fuerza = fuerzaBase;
        suerte = suerteBase;
        rafaga = rafagaBase;
        suerte = suerteBase;
        CambioEstadisticas?.Invoke();
    }

    public void AplicarEpifania(Epifania epifania)
    {
        vida += epifania.vida;
        velocidad += epifania.velocidad;
        aliento += epifania.aliento;
        aura += epifania.aura;
        prisa += epifania.prisa;
        alivio += epifania.alivio;
        fuerza += epifania.fuerza;
        suerte += epifania.suerte;
        rafaga += (epifania.rafaga/100) * rafagaBase;

        vida = Mathf.Clamp(vida, 10, 1000);
        aliento = Mathf.Clamp(aliento, 10, 1000);
        fuerza = Mathf.Clamp(fuerza, 0, 1000);
        aura = Mathf.Clamp(aura, 0, 1000);
        velocidad = Mathf.Clamp(velocidad, 20, 300);
        prisa = Mathf.Clamp(prisa, 0, 65);
        alivio = Mathf.Clamp(alivio, 0, 65);
        suerte = Mathf.Clamp(suerte, 0, 100);
        rafaga = Mathf.Clamp(rafaga, 0.8f, 5);
        CambioEstadisticas?.Invoke();
    }

    public void AumentarBase(StatBase stat, float valor)
    {
        switch (stat)
        {
            case StatBase.REGEN_ALIENTO:
                regenAliento += (int)valor;
                break;
            case StatBase.REGEN_VIDA:
                regenVida += (int)valor;
                break;
            case StatBase.VIDA:
                vidaBase += valor;
                break;
            case StatBase.ALIENTO:
                alientoBase += valor;
                break;
            case StatBase.VELOCIDAD:
                velocidadBase += valor;
                break;
            case StatBase.RAFAGA:
                rafagaBase += (valor / 100) * rafagaBase;
                break;
            case StatBase.SUERTE:
                suerteBase += valor;
                break;
            case StatBase.DMG:
                dmgBase += Mathf.RoundToInt(valor);
                break;
            case StatBase.ESCALADO_FUERZA:
                escaladoFuerza += valor / 100f;
                break;
            case StatBase.MULT_CRITICO:
                critMultiplier += valor / 100f;
                break;
            case StatBase.LIMITE_FORJAS:
                limiteForjasBase += Mathf.RoundToInt(valor);
                break;
            case StatBase.LIMITE_PODERES:
                limitePoderesBase += Mathf.RoundToInt(valor);
                break;
        }

        Reiniciar();
    }
}
public enum StatBase
{
    VIDA,
    ALIENTO,
    VELOCIDAD,
    DMG, RAFAGA, SUERTE,
    ESCALADO_FUERZA,
    REGEN_VIDA, REGEN_ALIENTO,
    MULT_CRITICO,
    LIMITE_FORJAS,
    LIMITE_PODERES
}