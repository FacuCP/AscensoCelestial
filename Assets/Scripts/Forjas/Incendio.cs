    using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Incendio : ForjaBase
{
    [SerializeField] private float dmgBase = 5;
    [SerializeField] private float dmgNivel = 2.5f;
    [SerializeField] private float dmgFuerza = 0.1f;
    [SerializeField] private float ralBase = 20f;
    [SerializeField] private float ralNivel = 2.5f;
    [SerializeField] private float ralFuerza = 0.05f;

    [SerializeField] private float fuerzaMax = 400f;
    [SerializeField] private float escalaMin = 1f;
    [SerializeField] private float escalaMax = 3f;
    private Collider col;
    private SpriteRenderer sprite;
    private LayerMask capaObjetivo;
    private bool activo = false;
    private Dictionary<Collider, float> ultimoTick = new Dictionary<Collider, float>();
    private float intervaloTick = 0.5f;

    public override void SetHandler(HandlerAtaque handler)
    {
        base.SetHandler(handler);
        transform.parent = null;

        // Obtener componentes automáticamente
        col = GetComponent<Collider>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        capaObjetivo = handler.CapaObjetivo;
        Desactivar(); // desactivar al inicio
    }

    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (!activo) {
            float t = Mathf.Clamp01(handler.Fuerza / fuerzaMax);
            float curva = Mathf.Log10(1 + 9 * t); // rápido al inicio, se aplasta al final
            float factor = Mathf.Lerp(escalaMin, escalaMax, curva);
            transform.localScale = Vector3.one * factor; // arranca en 1 y escala según fuerza
            transform.position = objetivo.transform.position; 
            StartCoroutine(ActivarTemporizado(5f)); 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        int capa = other.gameObject.layer;
        float ahora = Time.time;

        // Verificar si podemos aplicar el tick
        if (ultimoTick.TryGetValue(other, out float ultimo))
        {
            if (ahora - ultimo < intervaloTick) return; // aún no pasó el intervalo
        }
        ultimoTick[other] = ahora; // actualizar tiempo del último tick

        // Aplicar efecto solo si está activo y en la capa objetivo
        if (activo && ((1 << capa) & capaObjetivo) != 0)
        {
            HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
            HandlerEstados estados = other.GetComponent<HandlerEstados>();
            if (vida != null) { vida.RecibirDmgPorcentual(dmgBase + dmgNivel * (Nivel - 1) + dmgFuerza * handler.Fuerza, handler); }
            if (estados != null) { estados.AplicarRalentizado(intervaloTick, ralBase + ralNivel * (Nivel - 1) + ralFuerza * handler.Fuerza); }
        }
    }
    public override string GetDescripcion() { return "Al golpear, crea una zona de fuego bajo el objetivo que inflige daño a cualquier unidad que permanezca sobre ella."; }
    public override string GetNombre() { return "Incendio"; }
    // ---------------- FUNCIONES PRIVADAS ----------------
    private void Desactivar()
    {
        activo = false;
        if (sprite) sprite.enabled = false;
        if (col) col.enabled = false;
    }

    private void Activar()
    {
        activo = true;
        if (sprite) sprite.enabled = true;
        if (col) col.enabled = true;
    }

    private IEnumerator ActivarTemporizado(float duracion)
    {
        Activar();
        yield return new WaitForSeconds(duracion);
        Desactivar();
    }
}
