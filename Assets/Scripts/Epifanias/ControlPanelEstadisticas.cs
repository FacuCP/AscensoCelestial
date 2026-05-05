using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelEstadisticas : MonoBehaviour
{
    private Transform group;
    // Diccionario: Estadistica -> ControlLinea
    private Dictionary<Estadistica, ControlLinea> lineas;
    private TextMeshProUGUI tipoMejora;

    private void Awake()
    {
        // Encuentra el VerticalLayoutGroup en hijos
        var layout = GetComponentInChildren<VerticalLayoutGroup>(true);

        if (layout == null)
        {
            Debug.LogError("No se encontró VerticalLayoutGroup en hijos.");
            return;
        }

        group = layout.transform;
        tipoMejora = group.parent.GetComponentInChildren<TextMeshProUGUI>(true);

        // Mejor filtrarlo por nombre exacto:
        foreach (var tmp in group.parent.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.name == "Tipo Mejora")
            {
                tipoMejora = tmp;
                break;
            }
        }
        // Cargar ControlLinea
        lineas = new Dictionary<Estadistica, ControlLinea>();

        foreach (ControlLinea linea in group.GetComponentsInChildren<ControlLinea>(true))
        {
            if (!lineas.ContainsKey(linea.stat))
                lineas.Add(linea.stat, linea);
        }
    }

    public void Inicializar(Epifania epifania , TMP_ColorGradient gradient)
    {
        tipoMejora.text = "EPIFANIA " + epifania.tipo.ToTexto();
        tipoMejora.colorGradientPreset = gradient;
        ActualizarLinea(Estadistica.VIDA, epifania.vida);
        ActualizarLinea(Estadistica.ALIENTO, epifania.aliento);
        ActualizarLinea(Estadistica.VELOCIDAD, epifania.velocidad);
        ActualizarLinea(Estadistica.AURA, epifania.aura);
        ActualizarLinea(Estadistica.PRISA, epifania.prisa);
        ActualizarLinea(Estadistica.ALIVIO, epifania.alivio);
        ActualizarLinea(Estadistica.FUERZA, epifania.fuerza);
        ActualizarLinea(Estadistica.SUERTE, epifania.suerte);
        ActualizarLinea(Estadistica.RAFAGA, epifania.rafaga);
    }

    private void ActualizarLinea(Estadistica stat, int valor)
    {
        if (lineas.TryGetValue(stat, out ControlLinea linea))
        {
            string textoValor = valor == 0 ? "" : (valor > 0 ? $"+{valor}" : valor.ToString()); 
            if (stat == Estadistica.RAFAGA || stat == Estadistica.PRISA || stat == Estadistica.ALIVIO)
                textoValor += '%';
            linea.DarValor(textoValor);
        }
        else
        {
            Debug.LogWarning($"No se encontró ControlLinea para {stat}");
        }
    }

    private void ActualizarLinea(Estadistica stat, float valor)
    {
        if (lineas.TryGetValue(stat, out ControlLinea linea))
        {
            string textoValor = valor == 0 ? "" : (valor > 0 ? $"+{valor}" : valor.ToString());
            if (stat == Estadistica.RAFAGA || stat == Estadistica.PRISA || stat == Estadistica.ALIVIO)
                textoValor += '%';
            linea.DarValor(textoValor);
        }
        else
        {
            Debug.LogWarning($"No se encontró ControlLinea para {stat}");
        }
    }

}
