using TMPro;
using UnityEngine;

public class HUDNivel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI nivel, esencia;
    private void Start()
    {
        JugadorSM.Instancia.CambioEsencia += ActualizarEsencia;
        LevelManager.Instance.CambioNivel += Actualizar;
        Actualizar(TipoNivel.Base,0);
        ActualizarEsencia();
    }
    public void Actualizar(TipoNivel tipo, int nivel)
    {
        string valor = "Nivel ";
        switch (tipo) {
            case TipoNivel.Base:
                valor += "Base";
                break;
            case TipoNivel.Pelea:
                valor += nivel.ToString(); ;
                break;
            case TipoNivel.Descanso:
                valor = "Descanso";
                break;
            case TipoNivel.Jefe:
                valor = "Pelea Final";
                break;
        }
        this.nivel.text = valor;
    }

    public void ActualizarEsencia()
    {
        esencia.text = $"Esencia Angelical: {JugadorSM.Instancia.EsenciaAngelical}";
    }
}
