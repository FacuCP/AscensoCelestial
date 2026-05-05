using System;
using UnityEngine;


[System.Serializable]
public class Decreto
{
    [SerializeField] private int nivel = 0;
    private int nivelMaximo, costoOriginal, costo, aumentoCosto, costoTotal=0;
    private float mejoraBase, mejora, mejoraTotal = 0;
    [SerializeField] private TipoDecreto tipo;

    public int Nivel => nivel;
    public float Mejora => mejora;
    public float MejoraTotal => mejoraTotal;

    public int CostoTotal=>costoTotal;
    public int Costo => costo;

    public TipoDecreto Tipo => tipo;

    private int nivelOriginal;

    public Decreto(TipoDecreto tipo,int nivel, int costo, int aumentoCosto, float mejora)
    {
        this.tipo = tipo;
        nivelMaximo = nivel;

        this.costoOriginal = costo;
        this.costo = costo;
        this.aumentoCosto = aumentoCosto;

        this.mejora = mejora;
        nivelOriginal = this.nivel;
    }

    public void SubirNivel()
    {
        if (nivel + 1 <= nivelMaximo)
        {
            nivel++;
            JugadorSM.Instancia.ConsumirEsencia(costo);
            costoTotal += costo;
            costo += aumentoCosto;

            mejoraTotal = mejora * (nivel - nivelOriginal);
            mejoraBase += mejora;
        }
    }

    public void Cancelar()
    {
        JugadorSM.Instancia.AgregarEsencia(costoTotal);
        costoTotal = 0;
        costo = costoOriginal;
        mejoraTotal = 0;
        nivel = nivelOriginal;
    }

    public void Confirmar()
    {
        nivelOriginal = nivel;
        costoOriginal = costo;
        costoTotal = 0;
        mejoraTotal = 0;
    }

    public float ObtenerDiferencia()
    {
        return mejoraTotal;
    }

    public bool HabilitarMejora()
    {
        return nivel<nivelMaximo && costo<= JugadorSM.Instancia.EsenciaAngelical;
    }

    public override bool Equals(object obj)
    {
        return obj is Decreto decreto &&
               tipo == decreto.tipo;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(tipo);
    }

    public void SetearNivel(int nivel)
    {
        this.nivel = nivel;
        costo = costoOriginal + aumentoCosto * nivel;
        mejoraBase += mejoraBase * nivel; 
        mejoraTotal = mejora * (this.nivel - nivelOriginal);
    }
}

public enum TipoDecreto
{
    VIDA,
    ALIENTO,
    VELOCIDAD,
    REGEN_ALIENTO,
    REGEN_VIDA,

    RAFAGA,
    CRITICO,
    MULT_CRITICO,
    DMG_ATAQUE,

    EPICA, CELESTIAL, DIVINA, INFERNAL, EPI_FORJA, EPI_PODER,
    LIMITE_FORJA, LIMITE_PODER,

    PODER, FORJA,
}

public enum TipoPantallaDecreto
{
    BASICOS, ATAQUE, EPIFANIAS, HABILIDADES
}


