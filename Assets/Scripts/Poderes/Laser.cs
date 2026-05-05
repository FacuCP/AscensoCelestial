using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Laser : PoderBase
{

    [SerializeField] private float dmgBase = 10f;
    [SerializeField] private float escalado = 1f;
    [SerializeField] private float escaladoNivel = 5f;


    private LineRenderer line;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false; // empieza apagado
    }

    public override void Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        Vector3 direccion = (punto - origen).normalized;
        direccion.y = 0f; // opcional, si no querés que apunte en Y
        float rango = 1000f;

        // RaycastAll para golpear a TODOS
        RaycastHit[] hits = Physics.RaycastAll(origen, direccion, rango, capaObjetivo);

        Vector3 fin = origen + direccion * rango; // por defecto, hasta el rango máximo

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                OnImpacto(hit.collider, null);
                // buscamos el impacto más lejano para dibujar hasta ahí
                if (Vector3.Distance(origen, hit.point) > Vector3.Distance(origen, fin))
                    fin = hit.point;
            }
        }

        // Dibujar con LineRenderer
        line.SetPosition(0, origen);
        line.SetPosition(1, fin);
        line.enabled = true;

        // Reiniciamos el fade si ya hay uno corriendo
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut(0.2f)); // dura 0.2s
        IniciarEspera();
    }
    public override string GetDescripcion()
    {
        return "Dispara un rayo de energía que atraviesa todo a su paso, infligiendo un daño masivo y cegando a todo aquel que alcance.";
    }

    public override string GetNombre() { return "Chispa Final"; }
    private IEnumerator FadeOut(float duracion)
    {
        float tiempo = 0f;

        Gradient gradient = line.colorGradient;
        Color startColor = gradient.Evaluate(0f);
        Color endColor = gradient.Evaluate(1f);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = 1 - (tiempo / duracion);

            Color cStart = startColor; cStart.a = t;
            Color cEnd = endColor; cEnd.a = t;

            GradientColorKey[] colorKeys = {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            };
            GradientAlphaKey[] alphaKeys = {
                new GradientAlphaKey(cStart.a, 0f),
                new GradientAlphaKey(cEnd.a, 1f)
            };

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = newGradient;

            yield return null;
        }

        line.enabled = false;
    }

    protected override void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false)
    {
        base.OnImpacto(other, cuerpo, aliado);

        float dmg = GetDmg();
        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        if (vida != null) { 
            vida.RecibirDmg(dmg, Handler); 
        }
        HandlerEstados estados = other.GetComponentInChildren<HandlerEstados>();
        if(estados!=null){ estados.AplicarCiego(1 + 0.05f * this.handler.Aura); }
    }
    protected override float CalcularDmgBase()
    {
        float dmg = Handler.ModDmg * (dmgBase + escalado * this.Handler.Aura + escaladoNivel * this.Nivel);
        return dmg;
    }
}
