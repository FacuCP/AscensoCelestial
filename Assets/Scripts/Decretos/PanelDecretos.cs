using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PanelDecretos : MonoBehaviour
{
    public static PanelDecretos Instancia { get; private set; }
    [SerializeField] private TextMeshProUGUI titulo, esencia;

    private Dictionary<TipoPantallaDecreto, ControladorTipoDecreto> tiposDict;

    private void Awake()
    {
        // Singleton pattern robusto
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        // Diccionario con hijos inactivos
        var tiposArray = GetComponentsInChildren<ControladorTipoDecreto>(true);
        tiposDict = tiposArray.ToDictionary(t => t.Tipo, t => t);
    }

    // START: Estado inicial + eventos (JugadorSM ya está listo)
    private void Start()
    {
        gameObject.SetActive(false); // Estado inicial desactivado

        // Suscripción segura a eventos
        if (JugadorSM.Instancia != null)
        {
            JugadorSM.Instancia.CambioEsencia += ActualizarEsencia;
        }
    }

    // ONENABLE: Cada vez que se activa el panel
    private void OnEnable()
    {
        ActualizarEsencia();
    }

    // ONDISABLE: Limpieza de eventos
    private void OnDisable()
    {
        if (JugadorSM.Instancia != null)
        {
            JugadorSM.Instancia.CambioEsencia -= ActualizarEsencia;
        }
    }

    public void Activar()
    {
        if (!tiposDict.TryGetValue(AreaDecreto.TipoActual, out ControladorTipoDecreto tipo))
        {
            Debug.LogWarning($"Tipo de decreto {AreaDecreto.TipoActual} no encontrado");
            return;
        }

        titulo.text = tipo.ObtenerTitulo();

        Pausar.PausarJuego();
        gameObject.SetActive(true);

        tipo.Activar(); 
    }

    public void ActualizarEsencia()
    {
        if(JugadorSM.Instancia != null)
            esencia.text = $"Esencia Angelical: {JugadorSM.Instancia.EsenciaAngelical}";
    }

    public void Aceptar()
    {
        if (tiposDict.TryGetValue(AreaDecreto.TipoActual, out ControladorTipoDecreto tipo))
        {
            foreach (ControladorLineaDecreto linea in tipo.Lineas)
            {
                JugadorSM.Instancia.AplicarDecreto(linea.Decreto);
                linea.Guardar();
            }
            tipo.Desactivar();
        }
        List<Decreto> decretos = new List<Decreto>();
        foreach (ControladorTipoDecreto tipoD in tiposDict.Values)
        {
            tipoD.Activar();
        }
        LevelManager.Instance.GuardarJuego(); 
        foreach (ControladorTipoDecreto tipoD in tiposDict.Values)
        {
            tipoD.Desactivar();
        }
        gameObject.SetActive(false);
        Pausar.DespausarJuego();
    }

    public List<Decreto> GetDecretos()
    {
        List<Decreto> decretos = new List<Decreto>();
        foreach (ControladorTipoDecreto tipoD in tiposDict.Values)
        {
            decretos.AddRange(tipoD.GetDecretos());
        }
        return decretos;
    }

    public void CargarDecretos(Decreto[] decretos)
    {
        foreach (ControladorTipoDecreto tipoD in tiposDict.Values)
        {
            tipoD.Activar();

            foreach (ControladorLineaDecreto linea in tipoD.Lineas)
            {
                Decreto decretoActual = linea.Decreto;

                Decreto decretoGuardado = decretos
                    .FirstOrDefault(d => d.Equals(decretoActual));
                if (decretoGuardado == null)
                    continue;
                linea.Decreto.SetearNivel(decretoGuardado.Nivel);
                JugadorSM.Instancia.AplicarDecreto(linea.Decreto);
                linea.Guardar();
            }
            tipoD.Desactivar();
        }
    }

    public void Cancelar()
    {
        if (tiposDict.TryGetValue(AreaDecreto.TipoActual, out ControladorTipoDecreto tipo))
        {
            foreach (ControladorLineaDecreto linea in tipo.Lineas)
            {
                linea.Cancelar();
            }
            tipo.Desactivar();
        }
        gameObject.SetActive(false);
        Pausar.DespausarJuego();
    }

    public string ObtenerValorActual(TipoDecreto tipo)
    {
        if (JugadorSM.Instancia == null) return "";
        return tipo switch
        {
            TipoDecreto.VIDA => JugadorSM.Instancia.Stats.Vida.ToString(),
            TipoDecreto.ALIENTO => JugadorSM.Instancia.Stats.Aliento.ToString(),
            TipoDecreto.VELOCIDAD => (JugadorSM.Instancia.Stats.Velocidad * 10).ToString(),
            TipoDecreto.REGEN_VIDA => JugadorSM.Instancia.Stats.RegenVida.ToString(),
            TipoDecreto.REGEN_ALIENTO => JugadorSM.Instancia.Stats.RegenAliento.ToString(),
            TipoDecreto.RAFAGA => JugadorSM.Instancia.Stats.Rafaga.ToString(),
            TipoDecreto.CRITICO => JugadorSM.Instancia.Stats.Suerte.ToString(),
            TipoDecreto.MULT_CRITICO => JugadorSM.Instancia.Stats.CritMultiplier.ToString(),
            TipoDecreto.DMG_ATAQUE => JugadorSM.Instancia.Stats.DmgBase.ToString(),
            TipoDecreto.EPICA => CreadorEpifanias.ProbEpica.ToString(),
            TipoDecreto.CELESTIAL => CreadorEpifanias.ProbCelestial.ToString(),
            TipoDecreto.DIVINA => CreadorEpifanias.ProbDivina.ToString(),
            TipoDecreto.INFERNAL => CreadorEpifanias.ProbInfernal.ToString(),
            TipoDecreto.EPI_FORJA => CreadorEpifanias.ProbForja.ToString(),
            TipoDecreto.EPI_PODER => CreadorEpifanias.ProbPoder.ToString(),
            TipoDecreto.LIMITE_FORJA => JugadorSM.Instancia.Stats.LimiteForjas.ToString(),
            TipoDecreto.LIMITE_PODER => JugadorSM.Instancia.Stats.LimitePoderes.ToString(),
            TipoDecreto.PODER => "Poder",
            TipoDecreto.FORJA => "Forja",
            _ => tipo.ToString()
        };
    }
}
