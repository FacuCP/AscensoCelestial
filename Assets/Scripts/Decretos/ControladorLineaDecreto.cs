using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControladorLineaDecreto : MonoBehaviour
{
    [SerializeField] private TipoDecreto tipo;
    [SerializeField] private int costeBase, aumentoCoste, incremento, nivelMaximo;
    [SerializeField] Button botonMejorar;
    [SerializeField] private TMP_Text caracteristica, valor, mejora, coste;

    private PanelDecretos panel;
    private Decreto decreto;
    public Decreto Decreto => decreto;

    //AWAKE: Inicialización LOCAL (datos propios, objetos serializados)
    private void Awake()
    {
        decreto = new Decreto(tipo, nivelMaximo, costeBase, aumentoCoste, incremento);
    }

    //ONENABLE: Configuración cada vez que se activa (UI, referencias externas)
    private void OnEnable()
    {
        panel = GetComponentInParent<PanelDecretos>();
        if (panel != null)
        {
            Actualizar();
        }
        else
        {
            Debug.LogWarning($"PanelDecretos no encontrado en {name}", this);
        }
    }

    //START: Una sola vez al inicio (eventos, dependencias externas listas)
    private void Start()
    {
        SuscribirEventos();
        Actualizar();
    }

    private void OnDisable()
    {
        DesuscribirEventos(); //Evita memory leaks
    }

    private void SuscribirEventos()
    {
        if (JugadorSM.Instancia != null)
        {
            JugadorSM.Instancia.CambioEsencia += HabilitarBoton;
        }
    }

    private void DesuscribirEventos()
    {
        if (JugadorSM.Instancia != null)
        {
            JugadorSM.Instancia.CambioEsencia -= HabilitarBoton;
        }
    }

    public void Actualizar()
    {
        float valorAct;
        if (panel != null)
        {
            float.TryParse(panel.ObtenerValorActual(tipo), out valorAct);
            caracteristica.text = TipoDecretoExtensions.ObtenerCaracteristica(tipo);
            float value;
            if(tipo == TipoDecreto.RAFAGA && JugadorSM.Instancia?.Stats !=null)
            {
                float rafagaBase = JugadorSM.Instancia.Stats.RafagaBase;
                float rafaga = valorAct + (decreto.ObtenerDiferencia() / 100) * rafagaBase;
                value = rafaga;
            }
            else
            {
                value = valorAct + decreto.ObtenerDiferencia();
            }
            valor.text = (value).ToString("F2");

            coste.text = decreto.Costo.ToString();
            mejora.text = "+" + decreto.Mejora.ToString();
        }
    }

    public void HabilitarBoton()
    {
        botonMejorar.enabled = decreto!=null? decreto.HabilitarMejora():true;
    }

    public void Mejorar()
    {
        if (decreto.Costo <= JugadorSM.Instancia.EsenciaAngelical)
        {
            decreto.SubirNivel();
            Actualizar();
        }
    }
    public void Guardar()
    {
        decreto.Confirmar();
    }
    public void Cancelar()
    {
        decreto.Cancelar();
        Actualizar();
    }
    public void Activar()
    {
        gameObject.SetActive(true);
        Actualizar();
    }
    public void Desactivar()
    {
        gameObject.SetActive(false);
    }
}
public static class TipoDecretoExtensions
{
    public static string ObtenerCaracteristica(this TipoDecreto tipo)
    {
        return tipo switch
        {
            TipoDecreto.VIDA => "Vida Base",
            TipoDecreto.ALIENTO => "Aliento Base",
            TipoDecreto.VELOCIDAD => "Velocidad Base",
            TipoDecreto.REGEN_VIDA => "Vida por Nivel",
            TipoDecreto.REGEN_ALIENTO => "Regen. de Aliento",
            TipoDecreto.RAFAGA => "Ráfaga Base",
            TipoDecreto.CRITICO => "Crítico Base",
            TipoDecreto.MULT_CRITICO => "Daño Crítico",
            TipoDecreto.DMG_ATAQUE => "Daño de Ataque Base",
            TipoDecreto.EPICA => "Prob. de Épica",
            TipoDecreto.CELESTIAL => "Prob. de Celestial",
            TipoDecreto.DIVINA => "Prob. de Divina",
            TipoDecreto.INFERNAL => "Prob. de Infernal",
            TipoDecreto.EPI_FORJA => "Prob. de Forja",
            TipoDecreto.EPI_PODER => "Prob. de Poder",
            TipoDecreto.LIMITE_FORJA => "Límites de Forja",
            TipoDecreto.LIMITE_PODER => "Límites de Poder",
            TipoDecreto.PODER => "Poder",
            TipoDecreto.FORJA => "Forja",
            _ => tipo.ToString()
        };
    }
}
