using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PeleaMelControlador : MonoBehaviour
{
    private Camera camara;
    Conversacion[] dialogos;
    Conversacion[] dialogosSegundaFase;
    Conversacion[] dialogosMorirMel;
    [SerializeField] private Canvas healthBarMel;
    [SerializeField] private MelSM mel;
    [SerializeField] private Portal portal;

    [SerializeField] private GameObject pantallaWin;

    private CinemachineCamera cineCam;

    private CinemachineFollow follow;

    private Vector3 offsetOriginal;
    private Conversacion convActual;

    void Awake()
    {
        dialogos = CargarConversacionesInstanciadas("Dialogos/Dialogos/PeleaMel");
        dialogosSegundaFase = CargarConversacionesInstanciadas("Dialogos/Dialogos/SegundaFaseMel");
        dialogosMorirMel = CargarConversacionesInstanciadas("Dialogos/Dialogos/MuerteMel");

        healthBarMel.gameObject.SetActive(false);
    }

    private Conversacion[] CargarConversacionesInstanciadas(string path)
    {
        var originales = Resources.LoadAll<Conversacion>(path);
        var instancias = new Conversacion[originales.Length];

        for (int i = 0; i < originales.Length; i++)
        {
            instancias[i] = Instantiate(originales[i]);
            instancias[i].CargarEstado(); // importante si usás PlayerPrefs
        }

        return instancias;
    }

    private void OnEnable()
    {
        LevelManager.OnEscenaLista += Inicializar;
        AudioManager.Instance.StopMusica();
        AudioManager.Instance.SetClip(GameAssets.i.musicaJefe);
        DialogoController.FinDialogo += OnDialogoTerminado;
        portal.gameObject.SetActive(false);
        pantallaWin.SetActive(false);
    }

    private void OnDisable()
    {
        LevelManager.OnEscenaLista -= Inicializar;
        DialogoController.FinDialogo -= OnDialogoTerminado;
        camara.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
    }

    private void Inicializar()
    {
        camara = Camera.main;

        cineCam = FindFirstObjectByType<CinemachineCamera>();
        if (cineCam == null)
        {
            Debug.LogError("No se encontró CinemachineCamera");
            return;
        }

        follow = cineCam.GetComponent<CinemachineFollow>();
        if (follow == null)
        {
            Debug.LogError("La CinemachineCamera no tiene CinemachineFollow");
            return;
        }

        offsetOriginal = follow.FollowOffset;

        if (MelSM.Instance == null)
        {
            Debug.LogError("MelSM.Instance es null");
            return;
        }
        MelSM.Instance.SegundaFase += ActivarSegundaFase;
        MelSM.Instance.MurioJefa += ActivarMorir;

        Pausar.Detener();
        StartCoroutine(SecuenciaInicioPelea());
    }

    IEnumerator SecuenciaInicioPelea()
    {
        yield return StartCoroutine(EnfocarMel(2f));
        convActual = Conversacion.ObtenerConversacion(dialogos);
        DialogoController.Instancia.InicializarDialogo(convActual);
    }
    private void OnDialogoTerminado()
    {
        AudioManager.Instance.PlayMusicaConLoop(GameAssets.i.musicaJefe, 50f);
        StartCoroutine(SecuenciaFinDialogo());
    }

    IEnumerator SecuenciaFinDialogo()
    {
        yield return StartCoroutine(VolverACamaraJugador(2f));
        healthBarMel.gameObject.SetActive(true);
    }

    IEnumerator RotarCamaraQuaternion(float duracion)
    {
        Quaternion inicio = camara.transform.rotation;
        Quaternion fin = Quaternion.Euler(20f, 0f, 0f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            camara.transform.rotation = Quaternion.Slerp(inicio, fin, t);
            yield return null;
        }


        StartCoroutine(IniciarCombate());
    }

    IEnumerator IniciarCombate()
    {
        Pausar.Continuar();
        yield return new WaitForSeconds(2f);
        mel.IniciarCombate();
    }

    IEnumerator EnfocarMel(float duracion)
    {
        Transform jugador = JugadorSM.Instancia.transform; // el Follow real
        Vector3 offsetInicial = follow.FollowOffset;

        // Dirección desde el jugador hacia Mel
        Vector3 dir = mel.transform.position - jugador.position;

        // Offset deseado (ajustá estos valores a gusto)
        Vector3 offsetMel = offsetInicial + dir;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            follow.FollowOffset = Vector3.Lerp(offsetInicial, offsetMel, t);
            yield return null;
        }
    }

    IEnumerator VolverACamaraJugador(float duracion)
    {
        Vector3 inicio = follow.FollowOffset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            follow.FollowOffset = Vector3.Lerp(inicio, offsetOriginal, t);
            yield return null;
        }
        StartCoroutine(RotarCamaraQuaternion(1f));
    }

    private void ActivarSegundaFase()
    {
        StartCoroutine(SecuenciaSegundaFase());
    }

    IEnumerator SecuenciaSegundaFase()
    {
        Pausar.Detener();

        yield return StartCoroutine(EnfocarMel(1.5f));

        convActual = Conversacion.ObtenerConversacion(dialogosSegundaFase);
        DialogoController.Instancia.InicializarDialogo(convActual);

        // Esperar a que termine el diálogo
        bool dialogoTerminado = false;
        void Fin()
        {
            dialogoTerminado = true;
            DialogoController.FinDialogo -= Fin;
        }

        DialogoController.FinDialogo += Fin;

        yield return new WaitUntil(() => dialogoTerminado);

        yield return StartCoroutine(mel.CrecerCoroutine(1f));

        yield return StartCoroutine(VolverACamaraJugador(1.5f));

        // ACÁ recién empieza la segunda fase real
        mel.SpotlightManager.IniciarCicloLucesMoviles();

        Pausar.Continuar();

        yield return new WaitForSeconds(1f);

        mel.IniciarCombate();
    }

    private void ActivarMorir()
    {
        StartCoroutine(SecuenciaMorir());
    }

    IEnumerator SecuenciaMorir()
    {
        Pausar.Detener();

        yield return StartCoroutine(EnfocarMel(1.5f));

        convActual = Conversacion.ObtenerConversacion(dialogosMorirMel);
        DialogoController.Instancia.InicializarDialogo(convActual);

        // Esperar a que termine el diálogo
        bool dialogoTerminado = false;
        void Fin()
        {
            dialogoTerminado = true;
            DialogoController.FinDialogo -= Fin;
        }

        DialogoController.FinDialogo += Fin;

        yield return new WaitUntil(() => dialogoTerminado);

        yield return StartCoroutine(VolverACamaraJugador(1.5f));

        if (TrackerHistoria.Instancia.cantidadVictorias < TrackerHistoria.cantidadVictoriasJuego)
        { 
            yield return new WaitForSeconds(1f);
            JugadorSM.Instancia.Matar();
        }
        else
        {
            portal.gameObject.SetActive(true);
            Pausar.PausarJuego();
            pantallaWin.SetActive(true);
        }
    }

    public void CerrarPantalla() {
        pantallaWin.SetActive(false);
        Pausar.DespausarJuego();
    }

    public void CerrarJuego()
    {
        Application.Quit();
    }

}
