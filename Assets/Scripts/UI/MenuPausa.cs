using System.Collections;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa Instancia { get; private set; }

    [SerializeField] private PanelInformacion panelInfoPrefab;

    private PanelInformacion panelInfo= null;
    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (!PlayerPrefs.HasKey("yaInicio"))
        {
            // Primera ejecución
            PlayerPrefs.SetInt("yaInicio", 1);
            PlayerPrefs.Save();


            StartCoroutine(MostrarMenuConDelay());
        }
        else
        {
            // Ya se abrió antes
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MostrarMenuConDelay()
    {
        yield return new WaitForSeconds(0.8f);

        AparecerMenu();
        VentanaComoJugar();
    }

    public void ToggleMenu()
    {
        if (gameObject.activeSelf)
        {
            Continuar();
        }
        else
        {
            AparecerMenu();
        }
    }

    public void AparecerMenu()
    {
        Pausar.PausarJuego();
        gameObject.SetActive(true);
    }


    public void Continuar() {
        gameObject.SetActive(false);
        Pausar.DespausarJuego();
    }

    public void Configuracion() {
    }

    public void VentanaComoJugar()
    {
        if (panelInfo == null)
        {
            panelInfo = Instantiate(panelInfoPrefab, transform);
        }
        panelInfo.Activar(VentanaActiva.ComoJugar);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
