using UnityEngine;

public class Revitalizar : ForjaBase
{
    [SerializeField] private float porcentajeBase = 5;
    [SerializeField] private float porcentajeNivel = 1;
    [SerializeField] private float porcentajeFuerza = 0.01f;
    private HandlerVida vida;
    public override void SetHandler(HandlerAtaque handler) {
        base.SetHandler(handler);
        vida = Handler.GetComponent<HandlerVida>();
    }
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (vida != null) {
            vida.RecibirCura(GetValor(critico));
        }
    }

    private float GetValor(bool crit) {
        float dmg = Handler.Fuerza;
        dmg = crit ? dmg * Handler.Stats.CritMultiplier : dmg;
        return dmg * ((porcentajeBase + porcentajeNivel * Nivel) / 100 + porcentajeFuerza);
    }

    public override string GetDescripcion() { return "Recupera una parte de la vida equivalente al daño infligido."; }
    public override string GetNombre() { return "Revitalizar"; }
}
