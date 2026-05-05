using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [SerializeField] Slider barra;
    [SerializeField] HandlerVida handlerVida;
    [SerializeField] HandlerPoder handlerPoder;
    [SerializeField] HandlerEstados estados;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] private Image prefabImagenEstado;
    private Dictionary<TipoEstado, Image> imagenes = new();
    private HandlerFavores handlerFavores;
    private TextMeshProUGUI texto, titulo;
    [SerializeField] TipoBarra tipo = TipoBarra.VIDA;

    private void Start()
    {
        if (texto == null)
            texto = transform.Find("Valores")?.GetComponent<TextMeshProUGUI>();

        if (titulo == null)
            titulo = transform.Find("Titulo")?.GetComponent<TextMeshProUGUI>();
        texto = GetComponentInChildren<TextMeshProUGUI>();
        switch (tipo)
        {
            case TipoBarra.VIDA:
                if (handlerVida == null) { handlerVida = JugadorSM.Instancia.GetComponentInChildren<HandlerVida>(); }
                handlerVida.CambioVidaActual += setValorActual;
                handlerVida.CambioVidaMaxima += setValorMaximo;
                handlerVida.Conectar();
                break;
            case TipoBarra.ALIENTO:
                if (handlerPoder == null) { handlerPoder = JugadorSM.Instancia.GetComponentInChildren<HandlerPoder>(); }
                handlerPoder.CambioAlientoActual += setValorActual;
                handlerPoder.CambioAlientoMaximo += setValorMaximo;
                handlerPoder.Conectar();
                break;
            case TipoBarra.FAVOR:
                handlerFavores = JugadorSM.Instancia.GetComponentInChildren<HandlerFavores>();
                if (handlerFavores == null) return;
                handlerFavores.CambioCargaActual += setValorActual;
                handlerFavores.CambioCargaMaxima += setValorMaximo;
                handlerFavores.CambioFavor += setTitulo;
                gameObject.SetActive(false);
                break;
            default: break;
        }
    }
    private void OnEnable()
    {
        if (estados != null)
        {
            estados.OnInicioEstado += AgregarEstado;
            estados.OnFinEstado += QuitarEstado;
        }
    }
    private void OnDisable()
    {

        if (estados != null)
        {
            estados.OnInicioEstado -= AgregarEstado;
            estados.OnFinEstado -= QuitarEstado;
        }

        if (handlerVida != null)
        {
            handlerVida.CambioVidaActual -= setValorActual;
            handlerVida.CambioVidaMaxima -= setValorMaximo;
        }
        if (handlerPoder != null)
        {
            handlerPoder.CambioAlientoActual -= setValorActual;
            handlerPoder.CambioAlientoMaximo -= setValorMaximo;
        }
        /*if (handlerFavores != null)
        {
            handlerFavores.CambioCargaActual -= setValorActual;
            handlerFavores.CambioCargaMaxima -= setValorMaximo;
            handlerFavores.CambioFavor -= setTitulo;
        }*/
    }
    public void setValorActual(int valorActual)
    {
        barra.value = valorActual;
        if (texto != null) { texto.text = valorActual.ToString()+"/"+barra.maxValue.ToString(); }
    }

    public void setValorMaximo(int valorMaximo)
    {
        barra.maxValue = valorMaximo;
        if (texto != null) { texto.text = barra.value.ToString() + "/" + valorMaximo.ToString(); }
    }

    public void setTitulo(string valor)
    {
        gameObject.SetActive(valor!="");
        titulo?.SetText(valor);
    }


    public void AgregarEstado(TipoEstado estado)
    {
        if (imagenes.ContainsKey(estado)) return;

        if (!Datos.TryGetValue(estado, out var datos))
        {
            Debug.LogWarning($"No hay datos para el estado {estado}");
            return;
        }

        var gradiente = Resources.Load<TMPro.TMP_ColorGradient>(datos.RutaColor);
        var sprite = Resources.Load<Sprite>(datos.RutaSprite);

        if (gradiente == null || sprite == null)
        {
            Debug.LogError($"Error cargando recursos de {estado}");
            return;
        }

        Image nuevaImagen = Instantiate(prefabImagenEstado, rectTransform);
        nuevaImagen.sprite = sprite;
        nuevaImagen.color = gradiente.topLeft;

        imagenes[estado] = nuevaImagen;
    }

    public void QuitarEstado(TipoEstado estado)
    {
        if (!imagenes.TryGetValue(estado, out var img)) return;

        Destroy(img.gameObject);
        imagenes.Remove(estado);
    }

    private static readonly Dictionary<TipoEstado, DatoEstado> Datos = new()
{
    { TipoEstado.Invencible, new DatoEstado("Invencible", "Iconos Estados/Invencible", "Colores Estados/Invencible") },
    { TipoEstado.Inmortal, new DatoEstado("Inmortal", "Iconos Estados/Inmortal", "Colores Estados/Inmortal") },
    { TipoEstado.Imparable, new DatoEstado("Imparable", "Iconos Estados/Imparable", "Colores Estados/Imparable") },
    { TipoEstado.Invisible, new DatoEstado("Invisible", "Iconos Estados/Invisible", "Colores Estados/Invisible") },
    { TipoEstado.Ciego, new DatoEstado("Ciego", "Iconos Estados/Ciego", "Colores Estados/Ciego") },
    { TipoEstado.Paralizado, new DatoEstado("Paralizado", "Iconos Estados/Paralizado", "Colores Estados/Paralizado") },

    { TipoEstado.Maldito, new DatoEstado("Maldito", "Iconos Estados/Maldito", "Colores Estados/Maldito") },
    { TipoEstado.Bendito, new DatoEstado("Bendito", "Iconos Estados/Bendito", "Colores Estados/Bendito") },
    { TipoEstado.Ralentizado, new DatoEstado("Ralentizado", "Iconos Estados/Ralentizado", "Colores Estados/Ralentizado") },
    { TipoEstado.Acelerado, new DatoEstado("Acelerado", "Iconos Estados/Acelerado", "Colores Estados/Acelerado") },
    { TipoEstado.Debilitado, new DatoEstado("Debilitado", "Iconos Estados/Debilitado", "Colores Estados/Debilitado") },
    { TipoEstado.Empoderado, new DatoEstado("Empoderado", "Iconos Estados/Empoderado", "Colores Estados/Empoderado") },
};
}


public enum TipoBarra { VIDA, ALIENTO, FAVOR}