using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class HandlerVelocidad : MonoBehaviour
{
    [SerializeField] Estadisticas stats;
    [SerializeField] HandlerEstados estados;

    private float velocidad;
    public float Velocidad => velocidad;

    private void OnEnable()
    {
        ActualizarStats();
        stats.CambioEstadisticas += ActualizarStats;
        estados.OnAcelerado += Acelerar;
        estados.OnRalentizado += Ralentizar;
    }

    private void OnDisable()
    {
        stats.CambioEstadisticas -= ActualizarStats;
        estados.OnAcelerado -= Acelerar;
        estados.OnRalentizado -= Ralentizar;
    }

    private void ActualizarStats() {
        velocidad = stats.Velocidad;

        CambioVelocidad?.Invoke();
    }

    // Acelerar (se acumulan)
    private List<float> acelerarList = new List<float>();

    // Ralentizar (se queda con la más fuerte)
    private List<float> ralentizarList = new List<float>();

    public void Reiniciar()
    {
        StopAllCoroutines();
        acelerarList.Clear();
        ralentizarList.Clear();
    }

    // ---------------- ACELERAR ----------------
    public void Acelerar(float tiempo, float valor)
    {
        acelerarList.Add(valor);
        RecalcularVelocidad();
        StartCoroutine(RemoverAcelerar(tiempo, valor));
    }


    public event Action CambioVelocidad;
    private IEnumerator RemoverAcelerar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        acelerarList.Remove(valor);
        if (acelerarList.Count == 0) estados.FinEstado(TipoEstado.Acelerado);
        RecalcularVelocidad();
    }

    // ---------------- RALENTIZAR ----------------
    public void Ralentizar(float tiempo, float valor)
    {
        ralentizarList.Add(valor);
        RecalcularVelocidad();

        StartCoroutine(RemoverRalentizar(tiempo, valor));
    }

    private IEnumerator RemoverRalentizar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        ralentizarList.Remove(valor);
        if(ralentizarList.Count == 0) estados.FinEstado(TipoEstado.Ralentizado);
        RecalcularVelocidad();
    }

    // ---------------- RECÁLCULO ----------------
    private void RecalcularVelocidad()
    {
        float baseVelocidad = stats.Velocidad;

        // Aceleraciones acumuladas (sumamos porcentajes)
        float bonus = 0f;
        foreach (var a in acelerarList)
            bonus += a;

        // Ralentización más fuerte
        float slow = ralentizarList.Count > 0 ? Mathf.Max(ralentizarList.ToArray()) : 0f;

        // Fórmula: rafaga = base + (base * (bonus - slow))
        velocidad = baseVelocidad + (baseVelocidad * (bonus - slow));

        // Límite inferior
        velocidad = Mathf.Max(velocidad, stats.Velocidad * 0.30f);

        CambioVelocidad?.Invoke();
    }
}
