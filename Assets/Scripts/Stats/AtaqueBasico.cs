using System;
using UnityEngine;

public class AtaqueBasico : MonoBehaviour
{
    [SerializeField] private Transform padre; // el padre de la cápsula
    [SerializeField] private float radio = 0.5f; // distancia al padre
    [SerializeField] private float altura = 0.5f; // altura relativa al padre
    private LayerMask capaObjetivo;

    // Evento que notifica cuando golpea poderA un enemigo
    public event Action<GameObject> OnGolpe;

    public void SetCapaObjetivo(LayerMask capa)
    {
        capaObjetivo = capa;
    }

    public void MoverEnDireccion(Vector3 direccion)
    {
        if (direccion == Vector3.zero) return;
        Vector3 direccionPlano = direccion;
        direccionPlano.y = 0;

        if (direccionPlano == Vector3.zero) return;

        direccionPlano.Normalize();

        Vector3 nuevaPosicion = padre.position + direccionPlano * radio;
        nuevaPosicion.y = padre.position.y + altura;
        transform.position = nuevaPosicion;

        transform.rotation = Quaternion.LookRotation(direccionPlano, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & capaObjetivo) != 0)
        {
            OnGolpe?.Invoke(other.gameObject);
        }
    }
}