using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlPanelEpifania : MonoBehaviour
{
    private ControlPanelEstadisticas panelStats;
    private ControlPanelHabilidad panelHab;
    private Button botonSeleccion, botonExpandir;
    private Epifania epifania;

    public Sprite normalInfernal;
    public Sprite highlightedInfernal;
    public Sprite pressedInfernal;
    public Sprite selectedInfernal;
    public Sprite disabledInfernal;

    private Sprite imagenNormal;
    private SpriteState estadoNormal;
    private SpriteState estadoInfernal;
    private Image botonImagen;

    public TMP_ColorGradient gradienteNormal, gradienteInfernal;

    private void Awake()
    {
        panelStats = transform.Find("BotonSeleccion").Find("ContenidoMejora").Find("Estadisticas").GetComponent<ControlPanelEstadisticas>();
        panelHab = transform.Find("BotonSeleccion").Find("ContenidoMejora").Find("Habilidad").GetComponent<ControlPanelHabilidad>();
        Button boton = panelHab.GetComponentInParent<Button>();
        var botones = GetComponentsInChildren<Button>();
        botonSeleccion = botones.FirstOrDefault(b => b.name == "BotonSeleccion");
        botonExpandir = botones.FirstOrDefault(b => b.name == "BotonExpandir");
        estadoNormal = botonSeleccion.spriteState;
        estadoInfernal = new SpriteState();
        estadoInfernal.highlightedSprite = highlightedInfernal;
        estadoInfernal.pressedSprite = pressedInfernal;
        estadoInfernal.selectedSprite = selectedInfernal;
        estadoInfernal.disabledSprite = disabledInfernal;
        botonImagen = botonSeleccion.GetComponent<Image>();
        imagenNormal = botonImagen.sprite;
    }

    public void Inicializar(Epifania epifania)
    {
        Pausar.PausarJuego();
        this.epifania = epifania;
        botonExpandir.gameObject.SetActive(epifania.habilidad != null);
        TMP_ColorGradient grad;
        if (epifania.tipo == TipoEpifania.INFERNAL)
        {
            botonSeleccion.spriteState = estadoInfernal;
            botonImagen.sprite = normalInfernal;
            grad = gradienteInfernal;
        }
        else
        {
            botonSeleccion.spriteState=estadoNormal;
            botonImagen.sprite= imagenNormal;
            grad = gradienteNormal;
        }
        panelStats.Inicializar(epifania, grad);
        panelHab.Inicializar(epifania, grad);
    }

    public Action<Epifania> Elegida, Expandida;

    public void Seleccionada()
    {
        Elegida?.Invoke(epifania);
        EventSystem.current.SetSelectedGameObject(null);
        Pausar.DespausarJuego();
    }

    public void Expandir()
    {
        Expandida?.Invoke(epifania);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
