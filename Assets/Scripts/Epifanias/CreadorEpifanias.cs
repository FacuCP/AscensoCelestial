using JetBrains.Annotations;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public static class CreadorEpifanias
{
    public static Epifania GenerarVacia() { return ScriptableObject.CreateInstance<Epifania>(); }

    private static float probEpica=15, probCelestial=10, probDivina=5, probInfernal=3;
    private static float probPoder =15, probForja=25, probFavor=10;

    public static float ProbEpica => probEpica;
    public static float ProbCelestial => probCelestial;
    public static float ProbDivina => probDivina;
    public static float ProbInfernal => probInfernal;
    public static float ProbPoder => probPoder;
    public static float ProbForja => probForja;
    public static float ProbFavor => probFavor;
     

    private static int limiteMundana = 15, limiteEpica = 20, limiteCelestial = 25, limiteDivina = 30, limiteInfernal = 27;

    static readonly Estadistica[] STATS_NORMALES = {Estadistica.VIDA, Estadistica.VELOCIDAD, Estadistica.ALIENTO};

    static readonly Estadistica[] STATS_PODER ={Estadistica.AURA, Estadistica.ALIVIO, Estadistica.PRISA};

    static readonly Estadistica[] STATS_FORJA = {Estadistica.FUERZA, Estadistica.RAFAGA, Estadistica.SUERTE};

    public static Epifania Generar()
    {
        Epifania e = GenerarVacia();


        e.tipo = GenerarTipo();
        e.habilidad = e.tipo == TipoEpifania.INFERNAL? GenerarBlasfemia():GenerarHabilidadAleatoria(e.tipo);

        e = GenerarEstadisticas(e);

        return e;
    }

    private static Epifania GenerarEstadisticas(Epifania e)
    {
        int cantidadMejoras = 0, limite = 15;
        switch (e.tipo)
        {
            case TipoEpifania.INFERNAL:
                cantidadMejoras = Random.Range(5, 7);
                limite = limiteInfernal;
                break;
            case TipoEpifania.DIVINA:
                cantidadMejoras = Random.Range(5, 6);
                limite = limiteDivina;
                break;
            case TipoEpifania.CELESTIAL:
                cantidadMejoras = Random.Range(4, 5);
                limite = limiteCelestial;
                break;
            case TipoEpifania.EPICA:
                cantidadMejoras = Random.Range(3, 4);
                limite = limiteEpica;
                break;
            default: 
                cantidadMejoras = Random.Range(2, 3);
                limite = limiteMundana;
                break;
        }
        if(e.habilidad != null)
        {
            cantidadMejoras -= Random.Range(0, 1);
        }
        Estadistica[] estadisticasMejoradas = ObtenerEstadisticas(cantidadMejoras, e.habilidad);


        foreach (Estadistica est in estadisticasMejoradas)
        {
            switch (est) {
                case Estadistica.VIDA: e.vida = Random.Range(10, limite); break;
                case Estadistica.VELOCIDAD: e.velocidad = Random.Range(10, limite); break;
                case Estadistica.ALIENTO: e.aliento = Random.Range(10, limite); break;
                case Estadistica.AURA: e.aura = Random.Range(10, limite); break;
                case Estadistica.FUERZA: e.fuerza = Random.Range(10, limite); break;
                case Estadistica.SUERTE: e.suerte = Random.Range(10, limite); break;
                case Estadistica.PRISA: e.prisa = Random.Range(10, limite); break;
                case Estadistica.ALIVIO: e.alivio = Random.Range(10, limite); break;
                case Estadistica.RAFAGA: e.rafaga = Random.Range(10, limite); break;
            }
        }

        if(e.tipo == TipoEpifania.INFERNAL) e = GenerarEstadisticasNegativas(estadisticasMejoradas, e);

        return e;
    }

    private static Epifania GenerarEstadisticasNegativas(Estadistica[] est, Epifania e)
    {
        if (est == null || est.Length == 0)
            return e;

        // Cantidad de estadísticas negativas (aprox la mitad, con variación)
        int min = Mathf.Max(1, (est.Length / 2) - 1);
        int max = Mathf.Min(est.Length, (est.Length / 2) + 1);

        int cantidad = Random.Range(min, max + 1);

        // Pool para evitar repetir
        List<Estadistica> pool = new List<Estadistica>(est);

        for (int i = 0; i < cantidad; i++)
        {
            int index = Random.Range(0, pool.Count);
            Estadistica stat = pool[index];
            pool.RemoveAt(index);

            int valorNegativo = Random.Range(-limiteInfernal, -10);

            switch (stat)
            {
                case Estadistica.VIDA: e.vida = valorNegativo; break;
                case Estadistica.VELOCIDAD: e.velocidad = valorNegativo; break;
                case Estadistica.ALIENTO: e.aliento = valorNegativo; break;
                case Estadistica.AURA: e.aura = valorNegativo; break;
                case Estadistica.FUERZA: e.fuerza = valorNegativo; break;
                case Estadistica.SUERTE: e.suerte = valorNegativo; break;
                case Estadistica.PRISA: e.prisa = valorNegativo; break;
                case Estadistica.ALIVIO: e.alivio = valorNegativo; break;
                case Estadistica.RAFAGA: e.rafaga = valorNegativo; break;
            }
        }

        return e;
    }

    private static Estadistica[] ObtenerEstadisticas(int cantidad, Habilidad h)
    {
        List<Estadistica> pool = new List<Estadistica>();
        pool.AddRange(STATS_NORMALES);
        if (h != null)
        {
            switch (h.tipo)
            {
                case TipoHabilidad.PODER:
                    pool.AddRange(STATS_PODER);
                    break;

                case TipoHabilidad.FORJA:
                    pool.AddRange(STATS_FORJA);
                    break;

                default:
                    pool.AddRange(STATS_PODER);
                    pool.AddRange(STATS_FORJA);
                    break;
            }
        }
        else
        {
            // Si no hay habilidad, se habilitan todas
            pool.AddRange(STATS_PODER);
            pool.AddRange(STATS_FORJA);
        }

        // Evitar pedir más estadísticas de las disponibles
        cantidad = Mathf.Min(cantidad, pool.Count);

        // Selección aleatoria sin repetir
        List<Estadistica> seleccionadas = new List<Estadistica>();

        for (int i = 0; i < cantidad; i++)
        {
            int index = Random.Range(0, pool.Count);
            seleccionadas.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return seleccionadas.ToArray();
    }
    private static Habilidad GenerarHabilidadAleatoria(TipoEpifania tipo)
    {
        float total = 100;

        int tirada = Random.Range(0, (int)total);
        float roll = (float)tirada;

        if (roll < probPoder)
            return GenerarPoder();

        roll -= probPoder;
        if (roll < probForja)
            return GenerarForja();

        roll -= probForja;
        if (roll < probFavor && (tipo == TipoEpifania.DIVINA || tipo == TipoEpifania.CELESTIAL) )
            return GenerarFavor();

        return null;
    }

    private static Habilidad GenerarPoder() {
        List<Habilidad> todas = new List<Habilidad>();
        todas.AddRange(GameAssets.i.forjasEpifanias);
        int index = Random.Range(0, todas.Count);
        return todas[index];
    }

    private static Habilidad GenerarForja() {
        List<Habilidad> todas = new List<Habilidad>();
        todas.AddRange(GameAssets.i.poderesEpifanias);
        int index = Random.Range(0, todas.Count);
        return todas[index];
    }

    private static Habilidad GenerarBlasfemia()
    {
        List<Habilidad> todas = new List<Habilidad>();
        todas.AddRange(GameAssets.i.blasfemiasEpifanias);
        string blasfemiaJugador = JugadorSM.Instancia.GetBlasfemia();

        if (blasfemiaJugador != "")
        {
            foreach (Habilidad x in GameAssets.i.blasfemiasEpifanias)
            {
                if (blasfemiaJugador == x.GetNombre())
                {
                    return x;
                }
            }
        }
        int index = Random.Range(0, todas.Count);
        return todas[index];
    }

    private static Habilidad GenerarFavor()
    {
        List<Habilidad> todas = new List<Habilidad>();
        todas.AddRange(GameAssets.i.favoresEpifanias);
        string favorJugador = JugadorSM.Instancia.GetFavor();

        if (favorJugador != "")
        {
            foreach (Habilidad x in GameAssets.i.favoresEpifanias)
            {
                if (favorJugador == x.GetNombre())
                {
                    return x;
                }
            }
        }
        int index = Random.Range(0, todas.Count);
        return todas[index];
    }

    private static TipoEpifania GenerarTipo()
    {
        float total = 100;

        int tirada = Random.Range(0, (int)total);
        float roll = (float)tirada;

        if (roll < probEpica)
            return TipoEpifania.EPICA;

        roll -= probEpica;
        if (roll < probCelestial)
            return TipoEpifania.CELESTIAL;

        roll -= probCelestial;
        if (roll < probDivina)
            return TipoEpifania.DIVINA;

        roll -= probDivina;
        if (roll < probInfernal)
            return TipoEpifania.INFERNAL;

        return TipoEpifania.MUNDANA;
    }

    public static void AumentarProbabilidad(TipoEpifania stat, float valor)
    {
        switch (stat)
        {
            case TipoEpifania.EPICA: probEpica += valor;
                break;
            case TipoEpifania.CELESTIAL: probCelestial += valor;
                break;
            case TipoEpifania.DIVINA: probDivina += valor;
                break;
            case TipoEpifania.INFERNAL: probInfernal += valor;
                break;
            case TipoEpifania.PODER: probPoder += valor;
                break;
            case TipoEpifania.FORJA: probForja += valor;
                break;
        }
    }
}