using UnityEngine;

public class Cerebro : MonoBehaviour
{

    [SerializeField] private Rigidbody cuerpo;
    [SerializeField] private Estadisticas stats;
    [SerializeField] private HandlerEstados estados;
    private HandlerAtaque handlerAtaque;
    private HandlerFavores handlerFavores;
    private HandlerPoder handlerPoder;
    private HandlerVelocidad handlerVelocidad;
    private bool enMovimiento;
    private Vector3 direccion;

    private void Start()
    {
        direccion = Vector3.zero;
        handlerAtaque = stats.GetComponent<HandlerAtaque>();
        handlerPoder = stats.GetComponent<HandlerPoder>();
        handlerFavores = stats.GetComponent<HandlerFavores>();
        handlerVelocidad = stats.GetComponent<HandlerVelocidad>();
    }

    private void FixedUpdate()
    {
        Vector3 pos = cuerpo.position;
        if (!handlerAtaque.Atacando && !estados.Paralizado)
        {
            cuerpo.MovePosition(cuerpo.position + Time.fixedDeltaTime * handlerVelocidad.Velocidad * direccion);
        }
        enMovimiento =  pos != cuerpo.position;
    }

    public bool EnMovimiento => enMovimiento;
    public Vector3 Direccion => direccion;
    public void setDireccion(Vector3 dir)
    {
        direccion = dir.normalized;
    }

    public void LanzarAtaque(Vector3 direccion) { if(!estados.Ciego && !estados.Paralizado) handlerAtaque.Atacar(direccion); }

    public void LanzarPoder(Vector3 direccion) { if(!estados.Ciego && !estados.Paralizado) handlerPoder.Castear(direccion); }
    public void CambiarPoder(float siguiente) { handlerPoder.Cambio((int)(siguiente)); }
    public void LanzarBendicion(Vector3 direccion) { if (!estados.Ciego && !estados.Paralizado) handlerFavores.Castear(direccion); }
}
