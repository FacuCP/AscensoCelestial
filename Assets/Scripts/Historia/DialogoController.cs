using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogoController : MonoBehaviour
{
    public static DialogoController Instancia { get; private set; }
    private bool dialogoActivo;

    [SerializeField] private TextMeshProUGUI nombre, linea;
    [SerializeField] private Image imagen;
    public bool DialogoActivo => dialogoActivo;

    private Conversacion conversacionActual;
    private int indice;
    public static event Action FinDialogo;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        dialogoActivo = false;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }
    public void InicializarDialogo(Conversacion conversacion)
    {
        Pausar.PausarJuego();
        conversacionActual = conversacion;
        indice = 0;
        dialogoActivo = true;

        gameObject.SetActive(true);
        MostrarLinea();
    }

    public void Siguiente()
    {
        if (!dialogoActivo) return;

        indice++;

        if (indice >= conversacionActual.dialogos.Count)
        {
            FinalizarDialogo();
            return;
        }

        MostrarLinea();
    }

    private void MostrarLinea()
    {
        DialogoEntry dialogoActual = conversacionActual.dialogos[indice];
        nombre.text = dialogoActual.personaje.GetNombre();
        linea.text = dialogoActual.texto;
        imagen.sprite = dialogoActual.personaje.GetImagen();
    }

    private void FinalizarDialogo()
    {
        conversacionActual.Leer();
        dialogoActivo = false;
        gameObject.SetActive(false);
        conversacionActual = null;
        Pausar.DespausarJuego();
        FinDialogo?.Invoke();
    }
}
