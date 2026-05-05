using System.Collections;
using UnityEngine;

public class Celeridad : FavorBase
{
    private HandlerEstados estados;
    private HandlerPoder poder;
    [SerializeField] private float cantidadAliento = 10f;
    [SerializeField] private float tiempo = 4f;
    [SerializeField] private float mejoraAlientoNivel = 2.5f;
    [SerializeField] private float mejoraTiempoNivel = 0.25f;
    [SerializeField] private float fuerzaAcelerar = 15;
    [SerializeField] private float mejoraAcelerar = 5;
    [SerializeField] private float reduccionEspera = 0.5f;

    public override void Setup(StateMachine padre)
    {
        base.Setup(padre);
        poder = padre.GetComponentInChildren<HandlerPoder>();
        estados = padre.GetComponentInChildren<HandlerEstados>();
    }

    public override void Lanzar(Vector3 punto)
    {
        poder.ModificarAlientoProlongado(cantidadAliento + mejoraAlientoNivel, tiempo + mejoraTiempoNivel, false);
        estados.AplicarAcelerado(tiempo + mejoraTiempoNivel, fuerzaAcelerar + mejoraAcelerar);
        // Iniciamos la corutina
        StartCoroutine(AcelerarPoderesProlongado(tiempo + mejoraTiempoNivel));
    }

    private IEnumerator AcelerarPoderesProlongado(float duracion)
    {
        float tiempoRestante = duracion;

        while (tiempoRestante > 0)
        {
            foreach (var p in poder.Poderes)
            {
                p.Adelantar(reduccionEspera);
            }

            yield return new WaitForSeconds(0.5f);
            tiempoRestante -= 0.5f;
        }
    }

    public override string GetDescripcion()
    {
        return "Aumenta la velocidad del jugador y acelera la regeneración de aliento por segundo. Además, reduce significativamente los tiempos de recarga de sus habilidades.";
    }
    public override string GetNombre() { return "Celeridad"; }
}
