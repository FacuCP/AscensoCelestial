using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MelSM : BaseEnemySM
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static MelSM Instance { get; private set; }

    [Header("Spotlights")]
    [SerializeField] private SpotlightManager spotlightManager;
    public SpotlightManager SpotlightManager => spotlightManager;
    [SerializeField] private int cantLucesMoviles = 5;

    public int CantLucesMoviles => cantLucesMoviles;

    public Vector3 posicionInicial { get; private set; }

    public bool combateActivo {  get; private set; }
    public IniciarActo iniciar { get; private set; }
    public Bailar bailar { get; private set; }
    public Atacar atacar { get; private set; }
    public CerrarActo cerrar { get; private set; }

    private float velocidad = 10;
    public float Velocidad => velocidad;

    private HandlerVida handlerVida;
    private float inicioSegundaFase = 0.5f;

    public List<PoderBase> poderes;

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        TrackerHistoria.Instancia.PrimerEnfrentamientoMel();

        Cuerpo = GetComponent<Rigidbody>();
        posicionInicial = transform.position;
        iniciar = new IniciarActo(this);
        bailar = new Bailar(this);
        atacar = new Atacar(this);
        cerrar = new CerrarActo(this);

        Cuerpo = GetComponent<Rigidbody>();
        Stats = GetComponentInChildren<Estadisticas>();
        Estados = GetComponent<HandlerEstados>();
        Animador = GetComponentInChildren<HandlerAnimacion>();
        HandlerAtaque = GetComponentInChildren<HandlerAtaque>();
        HandlerPoder = GetComponentInChildren<HandlerPoder>();
        HandlerVelocidad = GetComponentInChildren<HandlerVelocidad>();
        handlerVida = GetComponentInChildren<HandlerVida>();
        combateActivo = false;

        foreach (PoderBase prefab in poderes)
        {
            PoderBase poder = Instantiate(prefab, transform);
            poder.gameObject.SetActive(true);
            poder.SetHandler(HandlerPoder);
            HandlerPoder.AgregarPoder(poder);
        }
        HandlerPoder.SetIndice(0);
        handlerVida.Murio += Morir;
        handlerVida.CambioVidaActual += CambioVida;
    }

    protected override BaseState GetInitialState() { return iniciar; }

    public void IniciarCombate()
    {
        combateActivo = true;
    }

    public void PausarCombate()
    {
        combateActivo = false;
    }

    public Action MurioJefa;

    protected override void Morir()
    {
        handlerVida.gameObject.SetActive(false);
        MurioJefa?.Invoke();
        TrackerHistoria.Instancia.Ganar();
        currentState = null;
    }

    public Action SegundaFase;
    public bool segundaFaseActivada { get; private set; } = false;
    public bool segundaFaseDisponible { get; private set; } = false;

    private void CambioVida(int vidaActual)
    {
        if (segundaFaseActivada) return;

        if (vidaActual <= handlerVida.Vida * inicioSegundaFase)
        {
            segundaFaseDisponible = true;
        }
    }

    public void ActivarFase()
    {
        PausarCombate();
        segundaFaseActivada = true;
        SegundaFase?.Invoke();
    }

    public IEnumerator CrecerCoroutine(float duracion)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        // Color objetivo #FFEEAF
        Color colorInicial = sr.color;
        Color colorFinal = new Color(1f, 238f / 255f, 175f / 255f);

        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaFinal = Vector3.one * 1.3f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;

            sr.color = Color.Lerp(colorInicial, colorFinal, t);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);

            yield return null;
        }

        // asegurar valores finales exactos
        sr.color = colorFinal;
        transform.localScale = escalaFinal;
    }
}
