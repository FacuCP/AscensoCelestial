using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DummyControler : MonoBehaviour
{
    public Vector3 direccion;
    [SerializeField] Cerebro handler;
    [SerializeField] CerebroBase cerebro;
    [SerializeField] private UnityEngine.Transform jugador;
    public enum Tipo
    {
        Ataque,
        Pulso,
        Poder,

    }

    [SerializeField] private Tipo tipo;
    private void Start()
    {
        if (tipo == Tipo.Poder) handler.CambiarPoder(1);
    }
    private void FixedUpdate()
    {
        switch (tipo)
        {
            case Tipo.Ataque:
                if(cerebro)cerebro.MoverHacia(jugador.position);
                //handler.LanzarAtaque(transform.position + direccion);
                break;
            case Tipo.Pulso:
                handler.LanzarPoder(transform.position + direccion);
                break;
            case Tipo.Poder:
                handler.LanzarPoder(transform.position + direccion*3);
                break;
            default:
                break;
        }

    }
}
