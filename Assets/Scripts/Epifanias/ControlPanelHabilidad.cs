using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControlPanelHabilidad : MonoBehaviour
{
    private Image imagen;
    private TextMeshProUGUI nombre, tipo;

    private void Awake()
    {
        imagen = GetComponentInChildren<Image>();
        nombre = transform.Find("Nombre Habilidad").GetComponent<TextMeshProUGUI>();
        tipo = transform.Find("Tipo Habilidad").GetComponent<TextMeshProUGUI>();
    }

    public void Inicializar(Epifania epifania, TMP_ColorGradient gradiente) {
        if(epifania.habilidad== null) { gameObject.SetActive(false); return; }
        gameObject.SetActive(true);
        nombre.text = epifania.habilidad.GetNombre();
        tipo.text = epifania.habilidad.tipo.ToTexto();

        nombre.colorGradientPreset = gradiente;
        tipo.colorGradientPreset = gradiente;
        nombre.ForceMeshUpdate();
        tipo.ForceMeshUpdate();

        imagen.sprite = epifania.habilidad.icono;
        imagen.color = gradiente.topLeft;
    }
}
