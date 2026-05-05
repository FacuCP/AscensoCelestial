using UnityEngine;

public class FuegoFatuo : PoderBase
{

    [SerializeField] private float dmgBase = 5;
    [SerializeField] private float dmgNivel = 1.5f;
    [SerializeField] private float dmgAura = 0.3f;
    [SerializeField] private float proyectilesAura = 40f;
    [SerializeField] private float anguloCono = 80f; // ángulo del cono en grados
    [SerializeField] private int minProyectiles = 3;
    [SerializeField] private int maxProyectiles = 5;

    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        if (!Disponible) return;

        // Dirección central
        Vector3 direccionCentral = (punto - origen).normalized;
        direccionCentral.y = 0f;

        // Cantidad de proyectiles
        int extraProyectiles = Mathf.FloorToInt(handler.Aura / proyectilesAura);
        int cantidad = 1 + extraProyectiles;

        // limitar a máximo
        cantidad = Mathf.Clamp(cantidad, minProyectiles, maxProyectiles);

        for (int i = 0; i < cantidad; i++)
        {
            // Distribución en el cono
            float factor = (cantidad == 1) ? 0.5f : i / (float)(cantidad - 1); // 0..1
            float anguloOffset = Mathf.Lerp(-anguloCono / 2f, anguloCono / 2f, factor);

            // Rotamos la dirección central para este proyectil
            Vector3 direccionProyectil = Quaternion.AngleAxis(anguloOffset, Vector3.up) * direccionCentral;

            // Llamamos al lanzar base usando la dirección rotada
            Vector3 destino = origen + direccionProyectil; // construimos un punto de referencia en esa dirección
            base.Lanzar(destino, origen, capaObjetivo, rotar);
            disponible = true;
        }

        // Iniciar cooldown
        IniciarEspera();
    }
    public override string GetDescripcion()
    {
        return "Dispara múltiples proyectiles que avanzan a lo largo de una trayectoria definida y, tras alcanzar su distancia máxima, regresan al punto de origen.";
    }

    public override string GetNombre() { return "Fuego Fatuo"; }
    protected override float CalcularDmgBase()
    { 
        return dmgBase + dmgNivel * this.Nivel + handler.Aura * dmgAura;
    }

    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        HandlerEstados estados = other.GetComponent<HandlerEstados>();
        float dmg = GetDmg();
        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
        {
            return;
        }
        vida.RecibirDmg(dmg, Handler);
    }

}
