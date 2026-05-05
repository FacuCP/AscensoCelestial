using UnityEngine;

public class Supernova : PoderBase
{
    [SerializeField] private float dmgBase = 30f;
    [SerializeField] private float escaladoAura = 0.5f;
    [SerializeField] private float escaladoNivel = 10f;
    [SerializeField] private float escaladoVidaPerdida = 0.5f;
    private float vidaPerdida = 0f;

    [SerializeField] private float escalaExplosion = 0.35f;
    [SerializeField] private float escalaDmg = 0.1f;

    [SerializeField] private float auraMax = 300f;
    [SerializeField] private float escalaMin = 1f;
    [SerializeField] private float escalaMax = 4f;

    [SerializeField] private float velocidadReduccion = 5f;
    [SerializeField] private float tiempoFade = 2.5f;

    [SerializeField] private float vidaBase = 100f;
    [SerializeField] private float escaladoVida = 0.30f;

    public float EscalaExplosion => escalaExplosion;
    public float EscalaDmg => escalaDmg;
    public float AuraMax => auraMax;
    public float EscalaMin => escalaMin;
    public float EscalaMax => escalaMax;
    public float VelocidadReduccion => velocidadReduccion;
    public float TiempoFade => tiempoFade;
    public float VidaBase => vidaBase;
    public float EscaladoVida => escaladoVida;

    public void Explotar(float vidaPerdida)
    {
        this.vidaPerdida = vidaPerdida;
        AudioManager.Instance.PlaySFX(clipGolpe,0.5f);
    }
    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        punto.y = punto.y + 0.2f;
        base.Lanzar(origen, punto, capaObjetivo, false);
    }
    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado =false)
    {
        HandlerVida v = other.GetComponentInChildren<HandlerVida>();
        if (v != null) { v.RecibirDmg(GetDmg(), this.Handler); }
    }

    protected override float CalcularDmgBase()
    {
        float dmg = dmgBase
            + escaladoAura * this.Handler.Aura
            + escaladoNivel * this.Nivel
            + escaladoVidaPerdida * vidaPerdida;

        return dmg;
    }
    public override string GetNombre() { return "Supernova"; }
    public override string GetDescripcion() { return "Genera una estrella en la ubicación seleccionada. Esta puede recibir daño y, al agotar su resistencia o transcurrido cierto tiempo, detona en una violenta explosión que inflige un gran daño a su alrededor."; }
}
