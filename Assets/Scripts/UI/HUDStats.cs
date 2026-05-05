using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDStats : MonoBehaviour
{
    private Estadisticas stats;
    private Transform group;

    // Diccionario: Estadistica -> ControlLinea
    private Dictionary<Estadistica, ControlLinea> lineas;

    private void Awake()
    {
        // Encuentra el VerticalLayoutGroup en hijos
        var layout = GetComponentInChildren<VerticalLayoutGroup>(true);
        stats = JugadorSM.Instancia.GetComponentInChildren<Estadisticas>();
        if (layout == null)
        {
            Debug.LogError("No se encontró VerticalLayoutGroup en hijos.");
            return;
        }

        group = layout.transform;
        // Cargar ControlLinea
        lineas = new Dictionary<Estadistica, ControlLinea>();

        foreach (ControlLinea linea in group.GetComponentsInChildren<ControlLinea>(true))
        {
            if (!lineas.ContainsKey(linea.stat))
                lineas.Add(linea.stat, linea);
        }
        stats.CambioEstadisticas += Inicializar;
    }

    private void Start()
    {
        Inicializar();
    }
    public void Inicializar()
    {
        ActualizarLinea(Estadistica.VIDA, stats.Vida);
        ActualizarLinea(Estadistica.ALIENTO, stats.Aliento);
        ActualizarLinea(Estadistica.VELOCIDAD, stats.VelocidadUI);
        ActualizarLinea(Estadistica.AURA, stats.Aura);
        ActualizarLinea(Estadistica.PRISA, stats.Prisa.ToString() + " %");
        ActualizarLinea(Estadistica.ALIVIO, stats.Alivio.ToString() + " %");
        ActualizarLinea(Estadistica.FUERZA, stats.Fuerza);
        ActualizarLinea(Estadistica.SUERTE, (stats.Suerte).ToString()+" %");
        ActualizarLinea(Estadistica.RAFAGA, $"{stats.Rafaga:F2} atq/s");
    }

    private void ActualizarLinea(Estadistica stat, int valor)
    {
        ActualizarLinea(stat, valor == 0 ? "" : valor.ToString());
    }
    private void ActualizarLinea(Estadistica stat, float valor)
    {
        ActualizarLinea(stat, Mathf.Approximately(valor, 0f) ? "0" : valor.ToString());
    }
    private void ActualizarLinea(Estadistica stat, string valor)
    {
        if (lineas.TryGetValue(stat, out ControlLinea linea))
        {
            linea.DarValorSimple(valor);
        }
        else
        {
            Debug.LogWarning($"No se encontró ControlLinea para {stat}");
        }
    }
}
