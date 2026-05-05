using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HandlerVida : MonoBehaviour
{
    [SerializeField] Estadisticas stats;
    [SerializeField] HandlerEstados estados;
    [SerializeField] float vidaElegida;

    private float vida;
    private float vidaActual;
    public float Vida => vida;
    public bool EstaVivo => vidaActual > 0; 
    public float VidaActual => vidaActual;

    private float modCura = 1f;
    private float modDmg = 1f;

    public event Action<int> RecibioDmg;
    public event Action<int> CambioVidaActual;
    public event Action<int> CambioVidaMaxima;
    public event Action Murio;

    public void SetVida(float vida)
    {
        this.vida = vida;
        this.vidaActual = vida;
        CambioVidaMaxima?.Invoke((int)vida);
        CambioVidaActual?.Invoke((int)vidaActual);
    }
    private void OnEnable()
    {

        if (stats == null)
        {
            vida = vidaElegida;
            vidaActual = vidaElegida;
            CambioVidaMaxima?.Invoke((int)vida);
            CambioVidaActual?.Invoke((int)vidaActual);
            return;
        }
        stats.CambioEstadisticas += ActualizarStats;
        estados.OnBendito += Bendecir;
        estados.OnMaldito += Maldecir;
        ActualizarStats();
    }

    private void OnDisable()
    {
        if (stats == null) { return; }
        stats.CambioEstadisticas -= ActualizarStats;
        estados.OnBendito -= Bendecir;
        estados.OnMaldito -= Maldecir;
    }

    private void ActualizarStats() {
        float vidaAdicional = stats.Vida - vida;
        vida = stats.Vida;
        vidaActual += vidaAdicional;
        Conectar();
    }

    public void Conectar()
    {
        CambioVidaMaxima?.Invoke((int)vida);
        CambioVidaActual?.Invoke((int)vidaActual);
    }
    public void Detener()
    {
        StopAllCoroutines();
    }
    public void Reiniciar() {
        Detener();
        modCura = 1;
        modDmg = 1;
        bendiciones= new List<float>();
        maldiciones = new List<float>();
        SetVida(stats.Vida);
    }

    public void Matar()
    {
        StopAllCoroutines();
        vidaActual = 0;
        CambioVidaActual?.Invoke((int)vidaActual);
        Murio?.Invoke();
    }

    private float CalcularVolumen(float valor)
    {
        return Mathf.Clamp(valor / 50,0.15f,1);
        
    }
    public void RecibirDmg(float cantidad, HandlerHabDmg handler, bool crit = false) {
        if ((estados && estados.Invencible)) return;
        if (vidaActual == 0) return;
        cantidad = cantidad * modDmg < 1f ? 1f : cantidad * modDmg;
        handler?.Golpeo(cantidad);

        AudioManager.Instance.PlaySFX(GameAssets.i.dmgTomado, CalcularVolumen(cantidad));

        vidaActual -= cantidad;
        RecibioDmg?.Invoke((int)vidaActual);
        if (estados) vidaActual = estados.Inmortal ? Mathf.Max(vidaActual, 1) : Mathf.Max(vidaActual, 0);
        else vidaActual = Mathf.Max(vidaActual, 0);
        CambioVidaActual?.Invoke((int)vidaActual);
        if (vidaActual == 0) { 
            Murio?.Invoke();
            AudioManager.Instance.PlaySFX(GameAssets.i.morir);
        }
        PopUpVida.Crear(transform.position, (int)cantidad, crit?TipoPopUp.Crit:TipoPopUp.Dmg);
    } 
    public void RecibirCura(float cantidad) {
        if (cantidad == 0) return;
        cantidad = cantidad * modCura < 1f ? 1f : cantidad * modCura;

        AudioManager.Instance.PlaySFX(GameAssets.i.dmgCurado, CalcularVolumen(cantidad));

        vidaActual += cantidad;
        vidaActual = Mathf.Min(vidaActual, vida);
        CambioVidaActual?.Invoke((int)vidaActual);
        PopUpVida.Crear(transform.position, (int)cantidad,TipoPopUp.Cura);
    }

    public void RecibirDmgPorcentual(float cantidad, HandlerHabDmg handler) {
        float valor = vidaActual * (cantidad / 100);
        this.RecibirDmg(Mathf.Clamp(valor,0,25), handler);
    }
    public void RecibirCuraPorcentual(float cantidad) {
        float valor = (vida - vidaActual) * (cantidad / 100);
        this.RecibirCura(valor);
    }

    public void RecibirDmgProlongado(float cantidad, HandlerHabDmg handler, float tiempo)
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(AplicarProlongado(() => RecibirDmg(cantidad, handler), tiempo));
    }

    public void RecibirCuraProlongado(float cantidad, float tiempo)
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(AplicarProlongado(() => RecibirCura(cantidad), tiempo));
    }

    public void RecibirDmgPorcentualProlongado(float cantidad, HandlerHabDmg handler, float tiempo)
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(AplicarProlongado(() => RecibirDmgPorcentual(cantidad, handler), tiempo));
    }

    public void RecibirCuraPorcentualProlongado(float cantidad, float tiempo)
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(AplicarProlongado(() => RecibirCuraPorcentual(cantidad), tiempo));
    }

    private IEnumerator AplicarProlongado(System.Action accion, float tiempo)
    {
        while (tiempo > 0)
        {
            if (this == null || gameObject == null)
                yield break;
            accion?.Invoke(); // Aplica la acción (daño/curación)
            yield return new WaitForSeconds(0.5f); // espera medio segundo
            tiempo -= 0.5f; // reduce el tiempo restante
        }
    }

    // Guardamos las bendiciones activas
    private List<float> bendiciones = new List<float>();

    // Guardamos las maldiciones activas
    private List<float> maldiciones = new List<float>();

    // ---------------- BENDICIÓN ----------------
    public void Bendecir(float tiempo, float valor)
    {
        bendiciones.Add(valor);
        RecalcularModCura();

        StartCoroutine(RemoverBendicion(tiempo, valor));
    }

    private IEnumerator RemoverBendicion(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        bendiciones.Remove(valor);
        if (bendiciones.Count == 0) estados.FinEstado(TipoEstado.Bendito);
        RecalcularModCura();
    }

    private void RecalcularModCura()
    {
        modCura = 1f; // base
        foreach (var b in bendiciones)
        {
            modCura += b; // se acumula curación extra
        }

        // Nueva parte: reduce daño recibido
        if (bendiciones.Count > 0)
        {
            modDmg = 1f - bendiciones.Sum(); // reducimos daño proporcional a bendiciones
            modDmg = Mathf.Max(0.1f, modDmg); // aseguramos que no llegue a 0
        }
    }

    // ---------------- MALDICIÓN ----------------
    public void Maldecir(float tiempo, float valor)
    {
        maldiciones.Add(valor);
        RecalcularModDmg();

        StartCoroutine(RemoverMaldicion(tiempo, valor));
    }

    private IEnumerator RemoverMaldicion(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        maldiciones.Remove(valor);
        if(maldiciones.Count == 0) estados.FinEstado(TipoEstado.Maldito);
        RecalcularModDmg();
    }

    private void RecalcularModDmg()
    {
        if (maldiciones.Count == 0)
        {
            modDmg = 1f; // sin maldición
        }
        else
        {
            modDmg = 1f + Mathf.Max(maldiciones.ToArray()); // efecto original
        }

        // Nueva parte: reduce curación recibida
        if (maldiciones.Count > 0)
        {
            modCura = 1f - maldiciones.Sum(); // reducimos curación proporcional a maldiciones
            modCura = Mathf.Max(0.1f, modCura); // aseguramos mínimo 10% de curación
        }
    }

}
