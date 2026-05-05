using UnityEngine;
using System.Collections;

public class SupernovaController : CuerpoPoder
{

    [SerializeField] private HandlerVida handlerVida;
    [SerializeField] private Animator animator;
    private Supernova prop;
    private Vector3 escalaInicial;
    private bool explotar = false;

    private void OnEnable()
    {
        if (handlerVida != null)
        {
            handlerVida.CambioVidaActual += ChequearEstado;
            handlerVida.Murio += Muerte;
        }
    }
    private void OnDisable()
    {
        if (handlerVida != null)
        {
            handlerVida.CambioVidaActual -= ChequearEstado;
            handlerVida.Murio -= Muerte;
        }
    }

    public override void Setup(Vector3 direccionInicial, PoderBase propietario, LayerMask capaObjetivo)
    {
        base.Setup(direccionInicial, propietario, capaObjetivo);
        gameObject.layer = 12;
        prop = (Supernova)propietario;
        float t = Mathf.Clamp01(propietario.Handler.Aura / prop.AuraMax);
        float curva = Mathf.Log10(1 + 9 * t); // rápido al inicio, se aplasta al final
        float factor = Mathf.Lerp(prop.EscalaMin, prop.EscalaMax, curva);
        transform.localScale = Vector3.one * factor; // arranca en 1 y escala según aura
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(1.3f, 3.4f, (transform.localScale.x - 1f) / (3.4f - 1f)), transform.position.z);
        escalaInicial = transform.localScale;
        handlerVida.SetVida(prop.VidaBase + prop.EscaladoVida * propietario.Handler.Aura);
    }

    private void ChequearEstado(int vidaActual)
    {
        if(prop == null) return;   
        float porcentajeVida = vidaActual/handlerVida.Vida; 
        if (porcentajeVida < 0.5) { 
            animator.SetBool("colapsoAzul", true);
        }
        if (porcentajeVida < 0.25) { 
            animator.SetBool("colapsoRojo", true);
        }
        transform.localScale = escalaInicial * (1+ prop.EscalaDmg * (1-porcentajeVida));
        if (porcentajeVida < 0.01) {
            explotar = true;
            CapsuleCollider c = gameObject.GetComponent<CapsuleCollider>();
            if (c != null) { c.enabled = false; }
            Rigidbody r = gameObject.GetComponent<Rigidbody>();
            if (r != null) { r.isKinematic = false; }
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Objetos";
            foreach (Transform hijo in transform)
            {
                if (hijo.name != "Sprite")
                {
                    hijo.gameObject.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!explotar) return;
        //SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //Transform t = spriteRenderer.transform;

        // decrecimiento exponencial
        transform.localScale *= Mathf.Exp(-prop.VelocidadReduccion * Time.deltaTime);

        // Cuando ya está en (casi) cero
        if (transform.localScale.magnitude <= 0.3f)
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            transform.position = new Vector3(transform.position.x,0.1f,transform.position.z);
            transform.localScale = escalaInicial * (1 + prop.EscalaExplosion);
            animator.SetBool("explosion", true);

            explotar = false; // para no repetir infinitamente
        }
    }
    public void Explotar()
    {
        SphereCollider s = gameObject.GetComponent<SphereCollider>();
        if (s != null)
        {
            prop.Explotar(handlerVida.Vida-handlerVida.VidaActual);
            StartCoroutine(ActivarColliderTemporal(s, 0.2f));
        }
    }

    public void Muerte()
    {
        handlerVida.Detener();    
    }

    private IEnumerator ActivarColliderTemporal(SphereCollider collider, float duracion)
    {
        collider.enabled = true;   
        collider.isTrigger = true;
        yield return new WaitForSeconds(duracion); 
        collider.enabled = false;
        collider.isTrigger = false;
    }
    public void FinExplosion()
    {
        StartCoroutine(FadeOut(prop.TiempoFade)); 
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

        // Aseguramos que quede completamente transparente
        spriteRenderer.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 0f);
        Destroy(gameObject);
    }
    public override void Reflejar() { }

}
