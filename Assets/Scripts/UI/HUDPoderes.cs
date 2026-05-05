using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HUDPoderes : MonoBehaviour
{
    [SerializeField] PanelPoder panelContenedor;
    [SerializeField] private RectTransform contenedorPadre;
    [SerializeField] private HandlerPoder handlerPoder;

    private List<PanelPoder> contenedores = new List<PanelPoder>();
    private float tamPanel;

    private static HUDPoderes instance;
    public static HUDPoderes Instance=>instance;

    private void Awake()
    {
        RectTransform rt = panelContenedor.GetComponent<RectTransform>();
        tamPanel = rt.rect.width;
        handlerPoder.CambioPoder += CambioPoder;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void CambioPoder(PoderBase poder)
    {
        if (poder == null)
        {
            Reiniciar();
            return;
        }
        PanelPoder existente = null;
        foreach (PanelPoder panelContenedor in contenedores)
        {
            if (panelContenedor.Poder == poder)
            {
                existente = panelContenedor;
                panelContenedor.SetPoder(poder);
                panelContenedor.PrimerPlano();
            }
            else
            {
                panelContenedor.SegundoPlano();
            }
        }
        // Si NO existe, lo creamos
        if (existente == null)
        {
            PanelPoder nuevo = Instantiate(panelContenedor, contenedorPadre);

            contenedores.Add(nuevo);

            // Ajustar tamaño
            contenedorPadre.sizeDelta = new Vector2(
                tamPanel * contenedores.Count,
                contenedorPadre.sizeDelta.y
            );

            int numero = contenedores.Count;

            nuevo.SetPoder(poder, numero);
        }
    }

    public void Reiniciar()
    {
        foreach (PanelPoder panelContenedor in contenedores)
        {
            Destroy(panelContenedor.gameObject);
        }
        contenedores = new List<PanelPoder>();
    }
}
