using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlLinea : MonoBehaviour
{
    public Estadistica stat;

    private TMP_ColorGradient gradiente;

    [SerializeField] private TextMeshProUGUI texto;
    [SerializeField] private TextMeshProUGUI valorTxt;
    [SerializeField] private Image imagen;


    public static readonly Dictionary<Estadistica, DatosEstadistica> Datos =
    new()
{
    { Estadistica.VIDA,     new DatosEstadistica("Vida", "Iconos Stats/iconoVida", "Colores Stats/Vida", "Cantidad total de vida del jugador. Al llegar a 0, el jugador muere y deberá comenzar nuevamente.") },
    { Estadistica.VELOCIDAD,  new DatosEstadistica("Velocidad", "Iconos Stats/iconoVelocidad", "Colores Stats/Velocidad", "Determina la rapidez con la que el jugador se desplaza.") },
    { Estadistica.ALIENTO,    new DatosEstadistica("Aliento", "Iconos Stats/iconoAliento", "Colores Stats/Aliento", "Recurso necesario para lanzar poderes. Se regenera mediante decretos o al pasar de nivel.") },
    { Estadistica.AURA,       new DatosEstadistica("Aura", "Iconos Stats/iconoAura", "Colores Stats/Aura", "Incrementa la potencia de los Poderes.") },
    { Estadistica.PRISA,      new DatosEstadistica("Prisa", "Iconos Stats/iconoPrisa", "Colores Stats/Prisa", "Reduce el tiempo de recarga de los Poderes.") },
    { Estadistica.ALIVIO,     new DatosEstadistica("Alivio", "Iconos Stats/iconoAlivio", "Colores Stats/Alivio", "Disminuye el costo de uso de los Poderes.") },
    { Estadistica.FUERZA,     new DatosEstadistica("Fuerza", "Iconos Stats/iconoFuerza", "Colores Stats/Fuerza", "Aumenta el daño de los Ataques cuerpo a cuerpo y de las Forjas.") },
    { Estadistica.RAFAGA,     new DatosEstadistica("Ráfaga", "Iconos Stats/iconoRafaga", "Colores Stats/Rafaga", "Incrementa la velocidad de ejecución de los Ataques cuerpo a cuerpo.") },
    { Estadistica.SUERTE,     new DatosEstadistica("Suerte", "Iconos Stats/iconoSuerte", "Colores Stats/Suerte", "Aumenta la probabilidad de que los Ataques cuerpo a cuerpo inflijan daño adicional.") },
};

    private void Awake()
    {
        if (texto == null || valorTxt == null || imagen == null)
        {
            Debug.LogError("ControlLinea: referencias no asignadas en el Inspector", this);
            return;
        }

        var datos = Datos[stat];

        gradiente = Resources.Load<TMP_ColorGradient>(datos.RutaColor);
        if (gradiente == null)
        {
            Debug.LogError($"Gradiente no encontrado: {datos.RutaColor}", this);
            return;
        }

        Sprite miSprite = Resources.Load<Sprite>(datos.RutaSprite);
        if (miSprite == null)
        {
            Debug.LogError($"Sprite no encontrado: {datos.RutaSprite}", this);
            return;
        }

        texto.colorGradientPreset = gradiente;
        valorTxt.colorGradientPreset = gradiente;
        imagen.color = gradiente.topLeft;
        imagen.sprite = miSprite;
    }

    public void DarValor(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Trim() == "%")
        {
            
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        string nombre = Datos[stat].Nombre;

        // Rellenar con espacios a la derecha hasta llegar al ancho
        string nombreAlineado = nombre;
        string valorAlineado = valor;
        texto.text = nombreAlineado;
        valorTxt.text = valorAlineado;
    }
    public void DarValorSimple(string valor)
    {
        // Construimos el nivel por código, sin depender del nivel original
        texto.text = valor;
        valorTxt.text = "";
    }

}
public class DatosEstadistica
{
    public string Nombre { get; }
    public string RutaSprite { get; }
    public string RutaColor { get; }
    public string Descripcion { get; }

    public DatosEstadistica(string nombre, string rutaSprite, string rutaColor)
    {
        Nombre = nombre;
        RutaSprite = rutaSprite;
        RutaColor = rutaColor;
    }

    public DatosEstadistica(string nombre, string rutaSprite, string rutaColor, string descripcion)
       : this(nombre, rutaSprite, rutaColor)
    {
        Nombre = nombre;
        RutaSprite = rutaSprite;
        RutaColor = rutaColor;
        Descripcion = descripcion;
    }
}
public enum Estadistica { VIDA,VELOCIDAD,ALIENTO,AURA,PRISA,ALIVIO,FUERZA,RAFAGA,SUERTE}
