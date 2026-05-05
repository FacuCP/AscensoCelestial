using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class AtaqueAcelerante : ForjaBase
{
    [SerializeField] float reduccionBase = 0.75f;
    [SerializeField] float reduccionNivel = 0.25f;
    [SerializeField] float tiempoBase = 0.3f;

    private HandlerPoder handlerPoderes;
    public override void SetHandler(HandlerAtaque handler) { 
        base.SetHandler(handler);
        handlerPoderes = handler.GetComponent<HandlerPoder>();
    }
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (handlerPoderes!= null){ 
            foreach (PoderBase poder in handlerPoderes.Poderes)
            {
                poder.Adelantar(reduccionBase + reduccionNivel * Nivel);
            } 
        }
    }
    public override string GetDescripcion() { return "Cada ataque reduce el tiempo de recarga de tus poderes."; }
    public override string GetNombre() { return "Ataque Acelerante"; }
}
