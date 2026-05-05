using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorTipoDecreto : MonoBehaviour
{
    [SerializeField] private TipoPantallaDecreto tipo;
    private ControladorLineaDecreto[] lineas;
    public ControladorLineaDecreto[] Lineas=>lineas;

    public TipoPantallaDecreto Tipo => tipo;

    // AWAKE: Inicialización LOCAL (GetComponentsInChildren)
    private void Awake()
    {
        Inicializar();
    }

    private void OnEnable()
    {
        if (lineas == null)
            Inicializar();
        ActivarLineas();
    }

    private void Inicializar()
    {
        lineas = GetComponentsInChildren<ControladorLineaDecreto>(true);
    }

    // START: Estado inicial (desactivado por defecto)
    public void Activar()
    {
        PanelDecretos panel = GetComponentInParent<PanelDecretos>(true);

        if (panel == null)
        {
            Debug.LogError("PanelDecretos no encontrado en los padres");
            return;
        }

        foreach (ControladorTipoDecreto hermano in panel.GetComponentsInChildren<ControladorTipoDecreto>(true))
        {
            if (hermano != this)
                hermano.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);

        ActivarLineas();
    }


    private void ActivarLineas()
    {
        if (lineas == null) return;
        foreach (ControladorLineaDecreto linea in lineas)
        {
            linea?.Activar();
        }
    }

    private void DesactivarLineas()
    {
        foreach (ControladorLineaDecreto linea in lineas)
        {
            linea?.Desactivar();
        }
    }

    public void Desactivar()
    {
        DesactivarLineas();
        gameObject.SetActive(false); //  Automáticamente llama OnDisable()
    }

    public string ObtenerTitulo()
    {
        return TipoPantallaDecretoExtensions.ObtenerCaracteristica(tipo);
    }

    public List<Decreto> GetDecretos()
    {
        List<Decreto> decretos = new();
        foreach (var linea in lineas) 
        {
            if (linea != null && linea.Decreto != null)
                decretos.Add(linea.Decreto);
        }

        return decretos;
    }
}

public static class TipoPantallaDecretoExtensions
{
    public static string ObtenerCaracteristica(this TipoPantallaDecreto tipo)
    {
        return tipo switch
        {
            TipoPantallaDecreto.BASICOS => "DECRETOS BASICOS",
            TipoPantallaDecreto.ATAQUE => "DECRETOS DE ATAQUE",
            TipoPantallaDecreto.EPIFANIAS => "DECRETOS DE EPIFANÍAS",
            TipoPantallaDecreto.HABILIDADES => "DECRETOS DE HABILIDADES",
            _ => tipo.ToString()
        };
    }
}
