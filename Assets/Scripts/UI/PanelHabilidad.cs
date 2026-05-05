using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelHabilidad : MonoBehaviour
{
    [SerializeField] Habilidad habilidad;

    [SerializeField] Image imagen;

    [SerializeField] TextMeshProUGUI nombre, descripcion, coste, espera;

    private void OnEnable()
    {
        if (habilidad != null) {
            imagen.sprite = habilidad.icono;
            nombre.text = habilidad.GetNombre();
            descripcion.text = habilidad.GetDescripcion();

            if (habilidad.tipo == TipoHabilidad.PODER)
            {
                coste.text = $"Coste: {habilidad.GetCoste()}";
                espera.text = $"Espera: {habilidad.GetEspera()} seg";
            }
            else if (habilidad.tipo == TipoHabilidad.FAVOR)
            {
                coste.text = $"Coste: {habilidad.GetCoste()}";
                espera.gameObject.SetActive(false);
            }
            else
            {
                coste.gameObject.SetActive(false);
                espera.gameObject.SetActive(false);
            }
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
