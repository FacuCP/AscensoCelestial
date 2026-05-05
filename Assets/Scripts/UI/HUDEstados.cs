using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDEstados : MonoBehaviour
{

    public static HUDEstados Instance { get; private set; }

    [SerializeField] private HandlerEstados estados;
    [SerializeField] private Image prefabImagenEstado;
    [SerializeField] private Transform contenedor;

    public static readonly Dictionary<TipoEstado, DatoEstado> Datos = new()
{
    { TipoEstado.Invencible, new DatoEstado("Invencible", "Iconos Estados/Invencible", "Colores Estados/Invencible", "El jugador no recibe daño ni puede ser afectado por estados negativos.") },
    { TipoEstado.Inmortal, new DatoEstado("Inmortal", "Iconos Estados/Inmortal", "Colores Estados/Inmortal", "El jugador no puede morir; su vida no se reduce por debajo de 1.") },
    { TipoEstado.Imparable, new DatoEstado("Imparable", "Iconos Estados/Imparable", "Colores Estados/Imparable", "El jugador es inmune a los efectos de estados negativos.") },
    { TipoEstado.Invisible, new DatoEstado("Invisible", "Iconos Estados/Invisible", "Colores Estados/Invisible", "El jugador no puede ser detectado por los enemigos.") },
    { TipoEstado.Ciego, new DatoEstado("Ciego", "Iconos Estados/Ciego", "Colores Estados/Ciego", "El jugador puede moverse, pero no puede atacar ni lanzar poderes.") },
    { TipoEstado.Paralizado, new DatoEstado("Paralizado", "Iconos Estados/Paralizado", "Colores Estados/Paralizado", "El jugador no puede moverse ni lanzar poderes.") },

    { TipoEstado.Maldito, new DatoEstado("Maldito", "Iconos Estados/Maldito", "Colores Estados/Maldito", "El jugador recibe más daño y sus efectos de curación son menos efectivos.") },
    { TipoEstado.Bendito, new DatoEstado("Bendito", "Iconos Estados/Bendito", "Colores Estados/Bendito", "El jugador recibe menos daño y sus efectos de curación son más efectivos.") },
    { TipoEstado.Ralentizado, new DatoEstado("Ralentizado", "Iconos Estados/Ralentizado", "Colores Estados/Ralentizado", "El jugador se mueve y ataca a menor velocidad.") },
    { TipoEstado.Acelerado, new DatoEstado("Acelerado", "Iconos Estados/Acelerado", "Colores Estados/Acelerado", "El jugador se mueve y ataca a mayor velocidad.") },
    { TipoEstado.Debilitado, new DatoEstado("Debilitado", "Iconos Estados/Debilitado", "Colores Estados/Debilitado", "El jugador inflige menos daño.") },
    { TipoEstado.Empoderado, new DatoEstado("Empoderado", "Iconos Estados/Empoderado", "Colores Estados/Empoderado", "El jugador inflige más daño.") },
};

    private Dictionary<TipoEstado, Image> imagenes = new();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        estados.OnInicioEstado += AgregarEstado;
        estados.OnFinEstado += QuitarEstado;
    }

    private void OnDisable()
    {
        estados.OnInicioEstado -= AgregarEstado;
        estados.OnFinEstado -= QuitarEstado;
    }

    public void AgregarEstado(TipoEstado estado)
    {
        // Evitar duplicados
        if (imagenes.ContainsKey(estado)) return;

        if (!Datos.TryGetValue(estado, out var datos))
        {
            Debug.LogWarning($"No hay datos para el estado {estado}");
            return;
        }

        // Cargar assets
        var gradiente = Resources.Load<TMPro.TMP_ColorGradient>(datos.RutaColor);
        var sprite = Resources.Load<Sprite>(datos.RutaSprite);

        if (gradiente == null || sprite == null)
        {
            Debug.LogError($"Error cargando recursos de {estado}");
            return;
        }

        // Instanciar UI
        Image nuevaImagen = Instantiate(prefabImagenEstado, contenedor);

        // Aplicar datos visuales
        nuevaImagen.sprite = sprite;
        nuevaImagen.color = gradiente.topLeft;

        // Guardar referencia
        imagenes[estado] = nuevaImagen;
    }
    public void QuitarEstado(TipoEstado estado)
    {
        if (!imagenes.TryGetValue(estado, out var img)) return;

        Destroy(img.gameObject);
        imagenes.Remove(estado);
    }

     public void Resetear()
    {
        foreach (var img in imagenes.Values)
        {
            if (img != null)
                Destroy(img.gameObject);
        }

        imagenes.Clear();
    }
}


public class DatoEstado
{
    public string Nombre { get; }
    public string RutaSprite { get; }
    public string RutaColor { get; }
    public string Descripcion { get; }

    public DatoEstado(string nombre, string rutaSprite, string rutaColor)
    {
        Nombre = nombre;
        RutaSprite = rutaSprite;
        RutaColor = rutaColor;
    }

    public DatoEstado(string nombre, string rutaSprite, string rutaColor, string descripcion)
     : this(nombre, rutaSprite, rutaColor)
    {
        Descripcion = descripcion;
    }
}


