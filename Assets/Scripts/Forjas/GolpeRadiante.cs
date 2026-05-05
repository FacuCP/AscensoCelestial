using UnityEngine;

public class GolpeRadiante : ForjaBase
{
    private int cargas = 0;
    [SerializeField] int maxCargas = 6;
    [SerializeField] float dmgBase = 15f;
    [SerializeField] float escaladoNivel = 2.5f;
    [SerializeField] float escaladoFuerza = 0.0f;
    [SerializeField] float dmgMaxCargas = 40f;
    [SerializeField] float dmgMaxCargasNivel = 10f;
    [SerializeField] float dmgMaxCargasFuerza = 1f;
    [SerializeField] float tiempoCegado = 2f;
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        HandlerVida vida = objetivo.GetComponentInChildren<HandlerVida>();
        if (cargas < maxCargas)
        {
            cargas++;
        }
        else
        {
            cargas = 0;
            HandlerEstados estadoObj = objetivo.GetComponentInParent<HandlerEstados>();
            if (estadoObj != null) estadoObj.AplicarCiego(tiempoCegado);
        }
        if (vida == null)
        {
            return;
        }
         vida.RecibirDmg(CalcularDmgBase(), handler);

    }
    private float CalcularDmgBase()
    {
        float dmg;
        if (cargas < maxCargas)
        {
            dmg = dmgBase + escaladoFuerza * Handler.Fuerza + escaladoNivel * Nivel;
        }
        else
        {
            dmg = dmgMaxCargas + dmgMaxCargasFuerza * Handler.Fuerza + dmgMaxCargasNivel * Nivel;
        }
        return Handler.ModDmg * dmg;
    }

    public override string GetDescripcion() { return "Cada ataque genera cargas que incrementan el daño infligido. Al alcanzar el máximo de cargas, se desencadena un golpe devastador que causa gran daño y ciega al objetivo."; }
    public override string GetNombre() { return "Golpe Radiante"; }
}
