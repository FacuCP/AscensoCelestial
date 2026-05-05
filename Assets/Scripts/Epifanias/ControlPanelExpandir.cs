using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelExpandir : MonoBehaviour
{

    private Button botonVolver;
    private Image imagen;

    [SerializeField] Sprite imagenInfernal;
    [SerializeField] TextMeshProUGUI nombre, coste, espera, descripcion;

    private Sprite imagenDivina;
    private void Awake()
    {
        imagen = GetComponent<Image>();
        imagenDivina = imagen.sprite;
    }

    public void Inicializar(Epifania epifania) { 
        this.gameObject.SetActive(true); 
        if(epifania.tipo == TipoEpifania.INFERNAL)
        {
            imagen.sprite =  imagenInfernal;
        }
        else
        {
            imagen.sprite = imagenDivina;
        }
        InicializarHabilidad(epifania.habilidad);
    }

    public void InicializarHabilidad(Habilidad habilidad)
    {
        if (habilidad != null)
        {
            nombre.text = habilidad.GetNombre();
            descripcion.text = habilidad.GetDescripcion();

            if (habilidad.tipo == TipoHabilidad.PODER)
            {
                coste.text = $"Coste: {habilidad.GetCoste()}";
                espera.text = $"Espera: {habilidad.GetEspera()} seg";
                coste.gameObject.SetActive(true);
                espera.gameObject.SetActive(true);
            }
            else if (habilidad.tipo == TipoHabilidad.FAVOR)
            {
                coste.text = $"Coste: {habilidad.GetCoste()}";
                coste.gameObject.SetActive(true);
                espera.gameObject.SetActive(false);
            }
            else
            {
                coste.gameObject.SetActive(false);
                espera.gameObject.SetActive(false);
            }
        }
    }

    public void Volver() { this.gameObject.SetActive(false); } 
}
