using System.Collections;
using UnityEngine;

public class AuraLuminosaController : CuerpoPoder
{
    private AuraLuminosa prop;
    private Vector3 escalaInicial;
    private bool encogiendo = false;
    private bool reaparecer = false;

    [SerializeField] private float auraMax = 1000f;
    [SerializeField] private float escalaMin = 1f;
    [SerializeField] private float escalaMax = 3f;

    [SerializeField] private float velocidadReduccion = 5f;
    [SerializeField] private float tiempoExtraConsumo = 1.5f;
    [SerializeField] private float tiempoFade = 2.5f;

    private Collider col;

    public float AuraMax => auraMax;
    public float EscalaMin => escalaMin;
    public float EscalaMax => escalaMax;
    public float VelocidadReduccion => velocidadReduccion;
    public float TiempoFade => tiempoFade;

    public override void Setup(Vector3 direccionInicial, PoderBase propietario, LayerMask capaObjetivo)
    {
        base.Setup(direccionInicial, propietario, capaObjetivo);
        gameObject.layer = 12;
        prop = (AuraLuminosa)propietario;

        // Escala inicial según aura
        float t = Mathf.Clamp01(propietario.Handler.Aura / prop.AuraMax);
        float curva = Mathf.Log10(1 + 9 * t);
        float factor = Mathf.Lerp(prop.EscalaMin, prop.EscalaMax, curva);
        transform.localScale = Vector3.one * factor;
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        escalaInicial = transform.localScale;
        col = GetComponent<Collider>();
        // Comenzamos el ciclo de encogimiento  reaparición  fade
        encogiendo = true;
    }

    private void FixedUpdate()
    {
        if (encogiendo)
        {
            // Encogerse con decrecimiento exponencial
            transform.localScale *= Mathf.Exp(-prop.VelocidadReduccion * Time.deltaTime);

            if (transform.localScale.magnitude <= 0.1f)
            {
                if (col != null) col.enabled = false;

                transform.localScale = escalaInicial;
                encogiendo = false;
                reaparecer = true;

                StartCoroutine(FinalizarConsumoConDelay(tiempoExtraConsumo));
            }
        }
        else if (reaparecer)
        {
            // Apenas reaparece, iniciamos el fade out
            StartCoroutine(FadeOut(prop.TiempoFade));
            reaparecer = false;
        }
    }

    private IEnumerator FinalizarConsumoConDelay(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        prop.SetConsumiendo(false);

        if (col != null)
            col.enabled = true;
    }

    private IEnumerator FadeOut(float duracion)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        Color colorInicial = spriteRenderer.color;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(colorInicial.a, 0f, tiempo / duracion);
            spriteRenderer.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alpha);
            yield return null;
        }

        // Aseguramos que quede invisible
        spriteRenderer.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 0f);
        Destroy(gameObject);
    }
    public override void Reflejar() { }
}
