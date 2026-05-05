using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

public class CuerpoPoder : MonoBehaviour
{
    private Rigidbody cuerpo;
    protected Vector3 direccion;
    protected PoderBase poderPropietario;
    protected LayerMask capaObjetivo;
    protected Vector3 posInicial;
    [SerializeField] protected float rango = 100;
    [SerializeField] private float duracion = 5;
    [SerializeField] protected float velocidad = 0;

    public PoderBase PoderPropietario => poderPropietario;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody>();
    }

    public virtual void Setup(Vector3 direccionInicial, PoderBase propietario, LayerMask capaObjetivo)
    {
        direccion = direccionInicial.normalized;
        poderPropietario = propietario;
        this.capaObjetivo = capaObjetivo;
        posInicial = this.transform.position;
        Destroy(gameObject,duracion);
    }

    public virtual void Reflejar()
    {
        direccion = -direccion;
        Quaternion rotacion = Quaternion.LookRotation(direccion) * Quaternion.Euler(0, 90, 0);
        transform.rotation = rotacion;

        bool esJugador = (capaObjetivo.value & LayerMask.GetMask("Jugador")) != 0;
        bool esEnemigo = (capaObjetivo.value & LayerMask.GetMask("Enemigo")) != 0;

        if (esJugador)
            capaObjetivo = LayerMask.GetMask("Enemigo");
        else if (esEnemigo)
            capaObjetivo = LayerMask.GetMask("Jugador");

        switch (gameObject.layer)
        {
            case 10: // HabilidadJugador
                gameObject.layer = 11; // HabilidadEnemigo
                break;
            case 11: // HabilidadEnemigo
                gameObject.layer = 10; // HabilidadJugador
                break;
            default:
                Debug.LogWarning("CuerpoPoder: layer inesperada al reflejar");
                break;
        }
    }

    private void FixedUpdate()
    {
        if (cuerpo != null)
        {
            cuerpo.MovePosition(cuerpo.position + Time.fixedDeltaTime * velocidad * direccion);
            float distancia = (cuerpo.transform.position - posInicial).magnitude;
            //if (distancia > rango) { Destroy(gameObject); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si la capa del objeto está dentro del LayerMask permitido
        if (((1 << other.gameObject.layer) & capaObjetivo.value) != 0)
        {
            poderPropietario.NotificarImpacto(other, this);
        }
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }
}