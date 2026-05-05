using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PanelEst : MonoBehaviour
{

    public Estadistica estadistica;
    public TipoEstado estado;

    public bool esEstado;


    [SerializeField] private TextMeshProUGUI nombre;
    [SerializeField] private TextMeshProUGUI descripcion;
    [SerializeField] private Image imagen;
    private void Awake()
    {
        if (esEstado)
            CargarEstado();
        else
            CargarEstadistica();
    }

    private void CargarEstado()
    {
        if (!HUDEstados.Datos.TryGetValue(estado, out var datos))
        {
            Debug.LogError($"No existe estado: {estado}", this);
            return;
        }

        AplicarVisual(datos.Nombre, datos.Descripcion, datos.RutaSprite, datos.RutaColor);
    }

    private void CargarEstadistica()
    {
        if (!ControlLinea.Datos.TryGetValue(estadistica, out var datos))
        {
            Debug.LogError($"No existe estadistica: {estadistica}", this);
            return;
        }

        AplicarVisual(datos.Nombre, datos.Descripcion, datos.RutaSprite, datos.RutaColor);
    }

    private void AplicarVisual(string nombreTxt, string descTxt, string rutaSprite, string rutaColor)
    {
        var sprite = Resources.Load<Sprite>(rutaSprite);
        var gradiente = Resources.Load<TMP_ColorGradient>(rutaColor);

        if (sprite == null || gradiente == null)
        {
            Debug.LogError("Error cargando recursos", this);
            return;
        }

        // SOLO la imagen cambia de color
        imagen.sprite = sprite;
        imagen.color = gradiente.topLeft;

        // Texto mantiene su color original
        nombre.text = nombreTxt;
        descripcion.text = descTxt;
    }
}
