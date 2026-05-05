using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JugadorSM : StateMachine
{
    public static JugadorSM Instancia { get; private set; }

    private int esenciaAngelical = 150;
    public int EsenciaAngelical => esenciaAngelical;

    public Action CambioEsencia;

    public void SetEsencia(int valor) { esenciaAngelical = valor; CambioEsencia?.Invoke(); }
    public void AgregarEsencia(int valor) { esenciaAngelical += valor;  CambioEsencia?.Invoke(); } 
    public void ConsumirEsencia(int valor) { esenciaAngelical -= valor;  CambioEsencia?.Invoke(); }

    public Vector3 direccion;

    public Rigidbody cuerpo;
    public HandlerEstados estados;
    public HandlerVelocidad velocidad;
    public HandlerAtaque ataque;
    public HandlerPoder poder;
    public HandlerFavores favor;
    public HandleBlasfemias blasfemia;
    public HandlerAnimacion animador;
    private HandlerVida vida;
    private Estadisticas estadisticas;

    public bool EstaVivo => vida.EstaVivo;

    public IdleJugador idle;
    public WalkJugador mover;
    public AtaqueJugador atacar;
    public HabsJugador habilidades;

    public List<PoderBase> poderesBase;
    private Dictionary<TipoDecreto, float> decretos;

    public Estadisticas Stats => estadisticas;


    private Conversacion[] dialogos;
    private Conversacion convActual;

    private void Awake()
    {
        // --- SINGLETON ---
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        // --- PERSISTENCIA ENTRE ESCENAS ---
        DontDestroyOnLoad(gameObject);
        idle = new IdleJugador(this);
        mover = new WalkJugador(this);
        atacar = new AtaqueJugador(this);
        habilidades = new HabsJugador(this);
        estadisticas = GetComponentInChildren<Estadisticas>();
        ataque = GetComponentInChildren<HandlerAtaque>();
        poder = GetComponentInChildren<HandlerPoder>();
        blasfemia = GetComponentInChildren<HandleBlasfemias>();
        favor = GetComponentInChildren<HandlerFavores>();
        animador = GetComponentInChildren<HandlerAnimacion>();
        cuerpo =GetComponent<Rigidbody>();

        estados = GetComponent<HandlerEstados>(); 
        velocidad = GetComponentInChildren<HandlerVelocidad>();
        vida = ataque.GetComponent<HandlerVida>();
        vida.Murio += Morir;


        foreach (PoderBase prefab in poderesBase)
        {
            PoderBase p = Instantiate(prefab, transform);
            p.gameObject.SetActive(true);
            p.SetHandler(poder);
            poder.AgregarPoder(p);
        }


        decretos = new Dictionary<TipoDecreto, float>();
        foreach (TipoDecreto tipo in System.Enum.GetValues(typeof(TipoDecreto)))
        {
            decretos[tipo] = 0f;
        }

        dialogos = CargarConversacionesInstanciadas("Dialogos/Dialogos/DialogosMuerte");

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

    protected override void Start()
    {
        base.Start();
        LevelManager.Instance.CargarJuego();
    }

    protected override BaseState GetInitialState() => idle;

    // DECRETOS

    public void AplicarDecreto(Decreto decreto)
    {
        float valor = decreto.MejoraTotal;

        switch (decreto.Tipo)
        {
            case TipoDecreto.VIDA: estadisticas.AumentarBase(StatBase.VIDA,valor);
                break;

            case TipoDecreto.ALIENTO: estadisticas.AumentarBase(StatBase.ALIENTO,valor);
                break;

            case TipoDecreto.VELOCIDAD: estadisticas.AumentarBase(StatBase.VELOCIDAD,valor);
                break;

            case TipoDecreto.REGEN_VIDA: estadisticas.AumentarBase(StatBase.REGEN_VIDA,valor);
                break;

            case TipoDecreto.REGEN_ALIENTO: estadisticas.AumentarBase(StatBase.REGEN_ALIENTO,valor);
                break;

            case TipoDecreto.RAFAGA: estadisticas.AumentarBase(StatBase.RAFAGA,valor);
                break;

            case TipoDecreto.CRITICO: estadisticas.AumentarBase(StatBase.SUERTE,valor);
                break;

            case TipoDecreto.MULT_CRITICO: estadisticas.AumentarBase(StatBase.MULT_CRITICO,valor);
                break;

            case TipoDecreto.DMG_ATAQUE: estadisticas.AumentarBase(StatBase.DMG,valor);
                break;

            case TipoDecreto.LIMITE_FORJA: estadisticas.AumentarBase(StatBase.LIMITE_FORJAS, valor);
                break;

            case TipoDecreto.LIMITE_PODER: estadisticas.AumentarBase(StatBase.LIMITE_PODERES, valor);
                break;
            case TipoDecreto.EPICA: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.EPICA,valor);
                break;
            case TipoDecreto.CELESTIAL: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.CELESTIAL,valor);
                break;
            case TipoDecreto.DIVINA: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.DIVINA,valor);
                break;
            case TipoDecreto.INFERNAL: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.INFERNAL,valor);
                break;
            case TipoDecreto.EPI_FORJA: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.FORJA,valor);
                break;
            case TipoDecreto.EPI_PODER: CreadorEpifanias.AumentarProbabilidad(TipoEpifania.PODER, valor);
                break;


            case TipoDecreto.PODER:
               // AplicarPoder(valor);
                break;

            case TipoDecreto.FORJA:
              //  AplicarForja(valor);
                break;
        }
        decretos[decreto.Tipo] = decreto.Nivel;
    }

    // ---- ENTRADAS DE INPUT ----

    public void OnInputMovimiento(Vector3 dir)
    {
        if (!Pausar.Pausado || !Pausar.Detenido) direccion = dir.normalized;
    }

    public void OnInputAtaque()
    {
        if (!Pausar.Pausado) currentState.OnAtaque();
    }

    public void OnInputPoder()
    {
        if (!Pausar.Pausado) currentState.OnPoder();
    }

    public void OnInputFavor()
    {
        if (!Pausar.Pausado) currentState.OnFavor();
    }

    public void OnInputCambio(float val)
    {
        if(!Pausar.Pausado)poder.Cambio((int)val);
    }

    public void OnInputNumero(int val)
    {
        if (!Pausar.Pausado) poder.Seleccionar(val);
     }
    private void Morir()
    {
        convActual = Conversacion.ObtenerConversacion(dialogos);
        DialogoController.Instancia.InicializarDialogo(convActual);
        LevelManager.Instance.Reiniciar();
        TrackerHistoria.Instancia.PrimeraMuerte();
    }

    public void FinNivel()
    {
        vida.StopAllCoroutines();
        vida.RecibirCura(Stats.RegenVida);
    }

    public void ReiniciarJugador()
    {
        direccion = Vector3.zero;
        estadisticas.Reiniciar();
        estados.Reiniciar();
        velocidad.Reiniciar();
        vida.Reiniciar();
        ataque.Reiniciar();
        poder.Reiniciar();
        blasfemia.Despojar();
        favor.Despojar(); 
        foreach (PoderBase prefab in poderesBase)
        {
            PoderBase p = Instantiate(prefab, transform);
            p.gameObject.SetActive(true);
            p.SetHandler(poder);
            poder.AgregarPoder(p);
        }
        // Resetear estadisticas
        // Despojarlo de Epifanias
        // Despojarlo de Decretos
        // Despojarlo de Poderes y de mas
        // Reiniciar Vida
        // Darle decretos

    }

    public string GetBlasfemia()
    {
        return blasfemia.Blasfemia?.GetNombre();
    }

    public string GetFavor() { return favor.Favor?.GetNombre(); }

    public void RecibirEpifania(Epifania epifania)
    {
        estadisticas.AplicarEpifania(epifania);
        if(epifania.habilidad == null) { return; }
        switch (epifania.habilidad.tipo) { 
            case TipoHabilidad.PODER:
                poder.AgregarPoderPrefab(epifania.habilidad.prefab); 
                break;
            case TipoHabilidad.FORJA:
                ataque.AgregarForjaPrefab(epifania.habilidad.prefab);
                break;
           case TipoHabilidad.FAVOR:
                favor.AgregarFavorPrefab(epifania.habilidad.prefab);
                break;
            case TipoHabilidad.BLASFEMIA:
                blasfemia.AgregarBlasfemiaPrefab(epifania.habilidad.prefab);
                break;
            default:break;
        }
    }

    public void Matar()
    {
        vida.Matar();
    }
}