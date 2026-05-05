using UnityEngine;

public class BrilloAngelical : PoderBase
{
    private int cargas = 0;
    private float ultimoImpacto = -999f;

    [Header("Cargas")]
    [SerializeField] private float tiempoRecarga = 1.2f;

    [Header("Daño")]
    [SerializeField] private float dmgBase = 30f;
    [SerializeField] private float dmgPorCarga = 10f;

    private int cantidadProyectiles = 1;
    private int proyectilesSegundaFase = 5;
    private int anguloCono = 120;

    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        if (!Disponible) return;

        // Dirección central
        Vector3 direccionCentral = (punto - origen).normalized;
        direccionCentral.y = 0f;

        int cantidad = cantidadProyectiles;

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

    private void Awake()
    {
        MelSM.Instance.SegundaFase += CambioFase;
    }
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);
        // Si pasó demasiado tiempo, se pierde la racha
        if (Time.time - ultimoImpacto > tiempoRecarga)
        {
            cargas = 0;
        }

        ultimoImpacto = Time.time;

        // Sumamos carga
        cargas++;

        float dmg = GetDmg();

        var vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null)
            return;

        vida.RecibirDmg(dmg, Handler);
        Destroy(cuerpo.gameObject);
    }

    protected override float CalcularDmgBase()
    {
        return dmgBase + cargas * dmgPorCarga;
    }

    private void CambioFase()
    {
        cantidadProyectiles = proyectilesSegundaFase;
    }

    public override string GetDescripcion()
    {
        return "";
    }

    public override string GetNombre()
    {
        return "";
    }
}
