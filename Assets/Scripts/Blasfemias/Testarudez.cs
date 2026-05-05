using UnityEngine;

public class Testarudez : BlasfemiaBase
{
    private HandlerEstados estados;
    [SerializeField] int cargasMax = 1;
    [SerializeField] float cargasNivel = 0.25f;
    private int cargasActuales;
    public override void Setup(Estadisticas stats)
    {
        base.Setup(stats);
        estados = stats.GetComponentInParent<HandlerEstados>();
        HandlerVida v = stats.GetComponent<HandlerVida>();
        v.RecibioDmg += Testarudo;
        cargasActuales =  cargasMax;
    }

    private void Testarudo(int vidaActual)
    {
        if (vidaActual <= 0 && cargasActuales > 0 )
        {
            estados.AplicarInmortal(10f);
            estados.AplicarImparable(10f);
            cargasActuales--;
        }
    }
    public override void AumentarNivel()
    {
        base.AumentarNivel();

        // Actualizamos la cantidad máxima de cargas
        cargasMax = Mathf.FloorToInt(1 + nivel * cargasNivel);

        // Recuperamos 1 carga
        cargasActuales = Mathf.Min(cargasActuales + 1, cargasMax);

    }


    public override string GetDescripcion()
    {
        return "Si recibes un golpe que debería matarte, en su lugar te vuelves inmortal e imparable durante un breve periodo de tiempo.";
    }
    public override string GetNombre() { return "Testarudez"; }
}
