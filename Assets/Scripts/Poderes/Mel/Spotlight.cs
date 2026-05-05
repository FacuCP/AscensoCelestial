using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spotlight : MonoBehaviour
{
    private bool haceDmg;
    private bool seMueve;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float poderEmpoderado;
    [SerializeField] float poderDebil;
    [SerializeField] float dmg;
    [SerializeField] float dmgStay;
    [SerializeField] float duracion;
    [SerializeField] float velocidadMovimiento = 10f;

    [SerializeField] LayerMask capaObjetivo;

    [SerializeField] AudioClip aparecer;
    [SerializeField] AudioClip desaparecer;

    public bool EsEspecial => haceDmg||seMueve;
    private SphereCollider area;
    private Vector3 centroArea;
    private float radioArea;
    public void Inicializar(bool dmgActivo, bool mov, SphereCollider areaMovimiento, Vector3 posicion)
    {
        Inicializar(dmgActivo, mov, areaMovimiento);
        if(EsEspecial)
            transform.position = posicion;
    }
    public void Inicializar(bool dmgActivo, bool mov,  SphereCollider areaMovimiento)
    {
        haceDmg = dmgActivo;
        seMueve = mov;

        area = areaMovimiento;
        centroArea = area.transform.position;
        radioArea = area.radius * area.transform.localScale.x;
        AudioManager.Instance.PlaySFX(aparecer,0.4f);
        if (haceDmg)
        {
            Color c = spriteRenderer.color;
            c.g = 0;
            spriteRenderer.color = c;
        }

        if (seMueve)
        {
            Color c = spriteRenderer.color;
            c.b = 0;
            c.g = 0.5f;
            spriteRenderer.color = c;
            StartCoroutine(Mover());
        }

        if (EsEspecial)
            StartCoroutine(AutoDestruir());

    }

    private IEnumerator AutoDestruir()
    {
        yield return new WaitForSeconds(duracion);
        Destruir();
    }

    private IEnumerator Mover()
    {
        Vector3 direccion = UnityEngine.Random.insideUnitSphere;
        direccion.y = 0f;
        direccion.Normalize();

        while (true)
        {
            Vector3 nuevaPos = transform.position + direccion * velocidadMovimiento * Time.deltaTime;

            float distancia = Vector3.Distance(
                new Vector3(nuevaPos.x, centroArea.y, nuevaPos.z),
                centroArea
            );

            if (distancia > radioArea)
            {
                // rebota hacia adentro
                direccion = (centroArea - transform.position).normalized;
            }
            else
            {
                transform.position = nuevaPos;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int capa = other.gameObject.layer;
        // Verificar si podemos aplicar el tick
        float ahora = Time.time;
        if (ultimoTick.TryGetValue(other, out float ultimo))
        {
            if (ahora - ultimo < intervaloTick) return; // aún no pasó el intervalo
        }
        ultimoTick[other] = ahora; // actualizar tiempo del último tick

        // Si está en la capa objetivo  impacto normal
        if (((1 << capa) & capaObjetivo.value) != 0)
        {
            OnEnter(other, true);
            return;
        }

        // Determinar la capa opuesta
        int capaOpuesta = (capaObjetivo.value == (1 << 7)) ? 8 : 7;

        // Si es la capa opuesta  impacto especial
        if (capa == capaOpuesta)
        {
            OnEnter(other, false);
        }
    }

    private void OnEnter(Collider other, bool enemigo)
    {
        if (other == null) return;

        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        HandlerEstados estados = other.GetComponent<HandlerEstados>();
        if (vida == null) return;

        if (enemigo)
        {
            if(haceDmg && seMueve)
            {
                vida.RecibirDmg(dmg, null);
            }
            if (estados != null)
            {
                estados.AplicarDebilitado(2f, poderDebil);
            }
        }
        else
        {
            if (estados != null)
            {
                estados.AplicarEmpoderado(2f, poderEmpoderado);
            }
        }

    }

    private Dictionary<Collider, float> ultimoTick = new Dictionary<Collider, float>();
    private float intervaloTick = 0.25f;

    

    public event Action<Spotlight> OnDestroyed;
    public void Destruir()
    {
        OnDestroyed?.Invoke(this);
        AudioManager.Instance.PlaySFX(desaparecer,0.05f);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!haceDmg) return;
        int capa = other.gameObject.layer;
        // Verificar si podemos aplicar el tick
        float ahora = Time.time;
        if (ultimoTick.TryGetValue(other, out float ultimo))
        {
            if (ahora - ultimo < intervaloTick) return; // aún no pasó el intervalo
        }
        ultimoTick[other] = ahora; // actualizar tiempo del último tick

        // Si está en la capa objetivo  impacto normal
        if (((1 << capa) & capaObjetivo.value) != 0)
        {
            OnStay(other);
            return;
        }
    }
    private void OnStay(Collider other)
    {
        if (other == null) return;

        HandlerVida vida = other.GetComponentInChildren<HandlerVida>();
        if (vida == null) return;
        vida.RecibirDmg(dmgStay, null);
        
    }
}