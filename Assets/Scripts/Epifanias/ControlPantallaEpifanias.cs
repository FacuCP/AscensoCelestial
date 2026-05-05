using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ControlPantallaEpifanias : MonoBehaviour
{
    public static ControlPantallaEpifanias Instancia { get; private set; }
    private ControlPanelEpifania[] paneles;
    private ControlPanelExpandir panelExpandir;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        var componente = GetComponentInChildren<HorizontalLayoutGroup>();
        panelExpandir = GetComponentInChildren<ControlPanelExpandir>();
        panelExpandir.gameObject.SetActive(false);
        paneles = GetComponentsInChildren<ControlPanelEpifania>();
    }

    public void Start()
    { 
        gameObject.SetActive(false);
    }
    public void Inicializar()
    {
        foreach (ControlPanelEpifania panel in paneles)
        {
            var nueva = CreadorEpifanias.Generar();
            panel.Inicializar(nueva);
            panel.Elegida -= Seleccion;
            panel.Expandida -= Expansion;
            panel.Elegida += Seleccion;
            panel.Expandida += Expansion;
        }
        gameObject.SetActive(true);
    }


    private void Seleccion(Epifania epifania)
    {
        JugadorSM.Instancia.RecibirEpifania(epifania);
        gameObject.SetActive(false);
    }

    private void Expansion(Epifania epifania)
    {
        panelExpandir.Inicializar(epifania);
    }
}
