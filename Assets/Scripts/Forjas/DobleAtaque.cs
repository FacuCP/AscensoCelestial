using UnityEngine;
using System.Collections;

public class DobleAtaque : ForjaBase
{
    private float dmgBase = 0.5f;
    [SerializeField] private float dmgNivel = 0.05f;
    private float dmgFuerza = 0.005f;
    [SerializeField] private float rafagaNueva = 5f; // golpea más rápido
    private int contador = 0;
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (tiempo == inicioAtaque) return;
        inicioAtaque = tiempo;
        if (aplica)
        {
            contador++;
            if (contador < 2) return;
            contador = 0;

            if (objetivo && handler != null)
            {
                handler.Animador.AtaqueReset += AtaqueExtraSeguro;
            }
        }
    }
    public override string GetDescripcion() { return "Cada cierta cantidad de ataques, se ejecuta un ataque secundario más débil que aplica forjas al objetivo."; }
    public override string GetNombre() { return "Doble Ataque"; }
    private void AtaqueExtraSeguro()
    {
        handler.Animador.AtaqueReset -= AtaqueExtraSeguro;
        handler.StartCoroutine(AtaqueExtraRutina());
    }
    private IEnumerator AtaqueExtraRutina()
    {
        var anim = handler.Animador.Animator;

        yield return new WaitForSeconds(0.05f);
    
        handler.Atacar(
            MouseController.GetMouseWorldPosition(),
            rafagaNueva,
            dmgBase + dmgNivel * (Nivel - 1) + dmgFuerza * handler.Fuerza
        );
    }

}
