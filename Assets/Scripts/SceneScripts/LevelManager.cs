using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DummyControler;
using Random = UnityEngine.Random;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    private bool isFading = false;
    private Canvas fadeCanvas;
    private static int nivel = 0;
    public static int Nivel => nivel;
    private TipoNivel tipoActual;
    private HUDNivel hudNivel;


    public event Action<TipoNivel, int> CambioNivel;
    public static event Action OnEscenaLista;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Crear Canvas/UI al inicio si no existe
        CreateFadeCanvas();
        fadeCanvasGroup.alpha = 1f; // Empezar visible

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void CreateFadeCanvas()
    {
        if (fadeCanvasGroup != null) return;

        // Crear Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 1000;

        CanvasGroup cg = canvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup = cg;

        // Crear Image negra
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        Image img = imageObj.AddComponent<Image>();
        img.color = Color.black;

        RectTransform rt = imageObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }

    public void LoadScene(TipoNivel tipo)
    {
        if (isFading) return;
        Pausar.Detener();
        AudioManager.Instance.PausarMusica();
        switch (tipo)
        {
            case TipoNivel.Base:
                nivel = 0;
                StartCoroutine(FadeOutAndLoad(1));
                AudioManager.Instance.SetClip(GameAssets.i.musicaMenu);
                break;
            case TipoNivel.Pelea:
                nivel = nivel +1;
                int indiceActual = SceneManager.GetActiveScene().buildIndex;
                int totalEscenas = SceneManager.sceneCountInBuildSettings;
                int nuevaEscena = Random.Range(4, totalEscenas);
                if (nuevaEscena == indiceActual)
                {
                    if (indiceActual + 1 < totalEscenas) nuevaEscena = indiceActual + 1;
                    else nuevaEscena = indiceActual - 1;
                }
                StartCoroutine(FadeOutAndLoad(nuevaEscena));
                AudioManager.Instance.SetClip(GameAssets.i.musicaNiveles);
                break;
            case TipoNivel.Descanso:
                nivel = 0;
                AudioManager.Instance.SetClip(GameAssets.i.musicaMenu);
                StartCoroutine(FadeOutAndLoad(2));
                break;
            case TipoNivel.Jefe:
                nivel = 0;
                StartCoroutine(FadeOutAndLoad(3));
                AudioManager.Instance.SetClip(GameAssets.i.musicaJefe);
                break;
        }
        tipoActual = tipo;
    }

    private bool reiniciarJugador=false;
    public void Reiniciar()
    {
        reiniciarJugador = true;
        tipoActual = TipoNivel.Base;
        nivel = 0;
        AudioManager.Instance.StopMusica();
        AudioManager.Instance.SetClip(GameAssets.i.musicaMenu);
        HUDEstados.Instance.Resetear();
        StartCoroutine(ReiniciarConFade());
    }

    private IEnumerator FadeOutAndLoad(int buildIndex)
    {
        if (isFading) yield break;
        isFading = true;

        fadeCanvasGroup.blocksRaycasts = false;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        SceneManager.LoadScene(buildIndex);
        if (JugadorSM.Instancia != null && reiniciarJugador)
        {
            JugadorSM.Instancia.ReiniciarJugador();
            reiniciarJugador = false;
        } 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode modo)
    {
        CambioNivel?.Invoke(tipoActual, nivel);
        // Esperar 1 frame + fade in
        StartCoroutine(FadeInDelayed());

        // Player spawn (después de 1 frame para que objetos existan)
        if(JugadorSM.Instancia!=null)StartCoroutine(RepositionPlayerDelayed());

        Pausar.Continuar();


        switch (tipoActual)
        {
            case TipoNivel.Base:
            case TipoNivel.Descanso:
                AudioManager.Instance.SetClip(GameAssets.i.musicaMenu);
                break;

            case TipoNivel.Pelea:
                AudioManager.Instance.SetClip(GameAssets.i.musicaNiveles);
                break;

            case TipoNivel.Jefe:
                AudioManager.Instance.SetClip(GameAssets.i.musicaJefe);
                break;
        }

        if (tipoActual != TipoNivel.Jefe)
            AudioManager.Instance.DespausarMusica();
    }

    private IEnumerator FadeInDelayed()
    {
        yield return null; // Esperar 1 frame
        CreateFadeCanvas(); // Asegurar que existe

        float elapsed = 0f;
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = false;
        while (elapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = true;
        isFading = false;

        OnEscenaLista?.Invoke();
    }

    private IEnumerator RepositionPlayerDelayed()
    {
        yield return null; // Esperar 1 frame
        GameObject player = JugadorSM.Instancia.gameObject;
        GameObject spawnpoint = GameObject.FindGameObjectWithTag("Respawn");
        if (player != null && spawnpoint != null)
            player.transform.position = spawnpoint.transform.position;
        else if (player != null)
            player.transform.position = Vector3.zero;
    }

    private IEnumerator ReiniciarConFade()
    {
        yield return StartCoroutine(FadeOutAndLoad(1));
        AudioManager.Instance.DespausarMusica();
    }

    public void GuardarJuego()
    {
        List<Decreto> decretos = PanelDecretos.Instancia.GetDecretos();
        int esencia = JugadorSM.Instancia.EsenciaAngelical;

        SaveLoadManagerJson.Instancia.SaveGame(decretos.ToArray(), esencia);
    }
    public void CargarJuego()
    {
        SaveData data = SaveLoadManagerJson.Instancia.LoadGame();
       // Debug.Log($"saveData = {data}");
       // Debug.Log($"saveData.decretos = {data?.decretos}");
        if (data == null) return;

        PanelDecretos.Instancia.CargarDecretos(data.decretos);
        JugadorSM.Instancia.SetEsencia(data.esencia);
    }
}
