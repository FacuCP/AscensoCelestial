using System.Collections;
using UnityEngine;

public class Frenesi : ForjaBase
{
    private HandlerEstados estados;
    private int cargas = 0;
    private bool enFrenesi = false;
    private Coroutine perderCargasCoroutine;
    [SerializeField] private float tiempoSinAtacar = 4;
    [SerializeField] private int cargasMax = 10;
    [SerializeField] private int cargasMin = 4;
    [SerializeField] private int redBase = 0;
    [SerializeField] private int redNivel = 1;
    [SerializeField] private float redFuerza = 0.01f;
    [SerializeField] private float velocidadBase = 20;
    [SerializeField] private float velocidadNivel = 5;
    [SerializeField] private float velocidadFuerza = 0.05f;
    [SerializeField] private float tiempoBase = 3;
    [SerializeField] private float tiempoNivel = 0.5f;
    [SerializeField] private float tiempoFuerza = 0.01f;

    public override void SetHandler(HandlerAtaque handler) { 
        base.SetHandler(handler);
        estados = GetComponentInParent<HandlerEstados>(); 
    }
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (enFrenesi) return; // no acumular cargas durante el efecto

        // Resetear timer de pérdida de cargas
        if (perderCargasCoroutine != null)
            StopCoroutine(perderCargasCoroutine);
        perderCargasCoroutine = StartCoroutine(PerderCargasTiempo());

        // Aumentar cargas
        cargas++;
        int cargasRed = Mathf.Clamp(cargasMax - redBase - redNivel * (Nivel - 1) - Mathf.FloorToInt(redFuerza * handler.Fuerza),cargasMin,cargasMax);
        if (cargas > cargasRed)
            cargas = cargasRed;

        // Aplicar efecto si llegamos al máximo
        if (cargas >= cargasRed)
        {
            cargas = 0;
            StartCoroutine(ActivarFrenesi());
        }

    }
    public override string GetDescripcion() { return "Cada ataque genera cargas. Al alcanzar el máximo, entras en estado de frenesí, el cual te acelera y te vuelve imparable."; }
    public override string GetNombre() { return "Frenesi"; }
    private IEnumerator ActivarFrenesi()
    {
        enFrenesi = true;
        float duracionFrenesi = tiempoBase + tiempoNivel * Nivel + handler.Fuerza * tiempoFuerza;
        float velocidad = velocidadBase + velocidadNivel * Nivel + handler.Fuerza * velocidadFuerza;
        estados.AplicarAcelerado(duracionFrenesi, 100); 
        estados.AplicarImparable(duracionFrenesi);

        yield return new WaitForSeconds(duracionFrenesi);

        enFrenesi = false;
    }

    private IEnumerator PerderCargasTiempo()
    {
        yield return new WaitForSeconds(tiempoSinAtacar);
        cargas = 0;
    }
}

