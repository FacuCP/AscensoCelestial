using UnityEngine;

public class FuegoFatuoController : CuerpoPoder
{
    private enum Estado { Avanzando, Regresando }
    private Estado estadoActual = Estado.Avanzando;

    [SerializeField] private float velocidadRegresoExtra = 2f; // velocidad extra al volver
    private Transform caster;

    public override void Setup(Vector3 direccionInicial, PoderBase propietario, LayerMask capaObjetivo)
    {
        base.Setup(direccionInicial, propietario, capaObjetivo);

        // Obtenemos al caster como el GameObject padre del propietario
        if (propietario.transform.parent != null)
        {
            caster = propietario.transform.parent;
        }
        else
        {
            caster = propietario.transform; // si no tiene padre, usamos el propio propietario
        }
    }

    private void FixedUpdate()
    {
        if (estadoActual == Estado.Avanzando)
        {
            // Movimiento normal
            transform.position += direccion * velocidad * Time.fixedDeltaTime;

            // Calcular distancia desde el inicio
            float distancia = (transform.position - posInicial).magnitude;
            if (distancia >= rango)
            {
                estadoActual = Estado.Regresando;
                velocidad += velocidadRegresoExtra; // aumentamos la velocidad al volver
            }
        }
        else if (estadoActual == Estado.Regresando)
        {
            if (caster != null)
            {
                // Recalculamos la dirección hacia el caster constantemente
                direccion = (caster.position - transform.position).normalized;

                // Movimiento hacia el caster
                transform.position += direccion * velocidad * Time.fixedDeltaTime;

                // Rotar el sprite para que apunte hacia la dirección de movimiento
                if (direccion != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direccion) * Quaternion.Euler(0, 90, 0);
                }

                // Si llega al caster, destruir el fuego
                if ((caster.position - transform.position).magnitude < 0.2f)
                {
                    Destruir();
                }
            }
            else
            {
                Destruir();
            }
        }
    }

}
