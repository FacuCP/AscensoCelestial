using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class  HandlerAtaque : HandlerHabDmg
{
    [SerializeField] Estadisticas stats;
    public Estadisticas Stats => stats;

    [SerializeField] HandlerAnimacion animador;// usar get component para repetidos
    public HandlerAnimacion Animador => animador;
    [SerializeField] AtaqueBasico area;
    [SerializeField] HandlerEstados estados;
    [SerializeField] private float modDmg = 1f;
    [SerializeField] private LayerMask capaObjetivo;

    public LayerMask CapaObjetivo => capaObjetivo;

    private float fuerza;
    private float suerte;
    private float rafaga;
    private int limiteForjas;

    private float dmgModifier = 1f;
    private bool ataquePrincipal = true;

    private List<ForjaBase> forjas = new List<ForjaBase>();

    private bool atacando;
    private float inicioAtaque;
    

    public float ModDmg => modDmg;
    public float Fuerza => fuerza;
    public bool Atacando => atacando;
    private void Start()
    {
        GetComponent<Estadisticas>();
        area.SetCapaObjetivo(capaObjetivo);
    }
    private void OnEnable()
    {
        ActualizarStats();

        area.gameObject.SetActive(false);
        stats.CambioEstadisticas += ActualizarStats;
        estados.OnAcelerado += Acelerar;
        estados.OnRalentizado += Ralentizar;
        estados.OnDebilitado += Debilitar;
        estados.OnEmpoderado += Empoderar;
        area.OnGolpe += Golpear;
        /*if (!CompareTag("Player"))
        {
            ForjaBase forja = Instantiate(GameAssets.i.forjas[0], transform);
            forja.gameObject.SetActive(true);
            forja.SetHandler(this);
            AgregarForja(forja);
        }*/
    }
    private void OnDisable()
    {
        stats.CambioEstadisticas -= ActualizarStats;
        estados.OnAcelerado -= Acelerar;
        estados.OnRalentizado -= Ralentizar;
        estados.OnDebilitado -= Debilitar;
        estados.OnEmpoderado -= Empoderar;
        area.OnGolpe -= Golpear;
    }

    private void ActualizarStats() { 
        fuerza = stats.Fuerza;
        suerte = stats.Suerte;
        rafaga = stats.Rafaga;
        limiteForjas = stats.LimiteForjas;
    }
    public void AgregarForjaPrefab(GameObject forjaPrefab)
    {
        if (forjaPrefab == null)
        {
            Debug.LogWarning("AgregarForjaPrefab recibió null");
            return;
        }

        // Instanciar
        GameObject instancia = Instantiate(forjaPrefab, transform);
        instancia.SetActive(true);

        // Obtener ForjaBase
        ForjaBase forja = instancia.GetComponent<ForjaBase>();
        if (forja == null)
        {
            Debug.LogError($"El prefab {forjaPrefab.name} no tiene un componente ForjaBase");
            Destroy(instancia);
            return;
        }

        // Enlazar al personaje
        forja.SetHandler(this);

        // Agregarla a la lista
        AgregarForja(forja);
    }

    public event Action<List<ForjaBase>> CambioForjas;
    public void AgregarForja(ForjaBase nuevaForja)
    {
        if (nuevaForja == null) return;
        if (!forjas.Contains(nuevaForja))
        {
            forjas.Add(nuevaForja);
        }
        else
        {
            int index = forjas.IndexOf(nuevaForja); // usa Equals()
            if (index != -1)
            {
                forjas[index].SubirNivel();
            }
        }
        CambioForjas?.Invoke(forjas);
    }
    public void Reiniciar()
    {
        StopAllCoroutines();
        forjas.Clear();
        dmgModifier = 1f;
        empoderarList.Clear();
        debilitarList.Clear();
        CambioForjas?.Invoke(forjas);
    }
    public void Atacar(Vector3 direccion, float speedMultiplier = 1f, float dmgMultiplier = 1f)
    {
        AudioManager.Instance.PlaySFX(GameAssets.i.castAtaque,0.5f);
        if (animador)
        {
            animador.AparecerCollider += ActivarCollider;
            animador.DesaparecerCollider += DesactivarCollider;
        }
        dmgModifier = dmgMultiplier;
        ataquePrincipal = speedMultiplier == 1? true : false;
        direccion = (direccion - transform.position).normalized;
        direccion = new Vector3(direccion.x, 0, direccion.z);

        if (animador) animador.Atacar(rafaga * speedMultiplier, direccion);
        inicioAtaque = Time.time;
        area.MoverEnDireccion(direccion);
    }
    private void ActivarCollider() { 
        area.gameObject.SetActive(true);
        animador.AparecerCollider -= ActivarCollider;
    }
    private void DesactivarCollider() { 
        area.gameObject.SetActive(false);
        animador.DesaparecerCollider -= DesactivarCollider;
    }

    // Método que se llama cuando AtaqueBasico detecta
    private float GetDmg(out bool critico)
    {
        critico = false;

        // daño base
        float dmg = (stats.DmgBase + stats.EscaladoFuerza * fuerza) * modDmg * dmgModifier;

        // chequeo crítico
        if (Random.Range(0f, 100f) < suerte)
        {
            critico = true;
            dmg *= stats.CritMultiplier;
        }

        return dmg;
    }
    private void Golpear(GameObject objetivo)
    {
        HandlerVida target = objetivo.GetComponentInChildren<HandlerVida>();

        bool critico = false;
        if (target)
        {
            float dmg = GetDmg(out critico);
            target.RecibirDmg(dmg, this, critico);
        }

        foreach (var forja in forjas)
        {
            forja.Lanzar(objetivo, capaObjetivo, ataquePrincipal, critico, inicioAtaque);
        }
    }

    // Empoderar (se acumulan)
    private List<float> empoderarList = new List<float>();

    // Debilitar (toma solo el más fuerte)
    private List<float> debilitarList = new List<float>();

    // ---------------- EMPODERAR ----------------
    public void Empoderar(float tiempo, float valor) 
    {
        empoderarList.Add(valor);
        RecalcularModDmg();

        StartCoroutine(RemoverEmpoderar(tiempo, valor));
    }

    private IEnumerator RemoverEmpoderar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        empoderarList.Remove(valor);
        if (empoderarList.Count == 0) estados.FinEstado(TipoEstado.Empoderado);
        RecalcularModDmg();
    }

    // ---------------- DEBILITAR ----------------
    public void Debilitar(float tiempo, float valor)
    {
        debilitarList.Add(valor);
        RecalcularModDmg();

        StartCoroutine(RemoverDebilitar(tiempo, valor));
    }

    private IEnumerator RemoverDebilitar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        debilitarList.Remove(valor);
        if(debilitarList.Count == 0) estados.FinEstado(TipoEstado.Debilitado);
        RecalcularModDmg();
    }

    // ---------------- RECÁLCULO ----------------
    private void RecalcularModDmg()
    {
        float mejoras = 0f;

        // Empoderar se acumula (ej: +0.2 +0.5 = +0.7)
        foreach (var e in empoderarList)
            mejoras += e;

        // Debilitar toma el más fuerte (ej: -0.2 o -0.3)
        float debuff = debilitarList.Count > 0 ? Mathf.Max(debilitarList.ToArray()) : 0f;

        // Multiplicamos ambos efectos
        modDmg = 1 + mejoras - debuff;
        GetComponent<HandlerPoder>().SetModDmg(modDmg);
    }

    // Acelerar (se acumulan)
    private List<float> acelerarList = new List<float>();

    // Ralentizar (se queda con la más fuerte)
    private List<float> ralentizarList = new List<float>();

    // ---------------- ACELERAR ----------------
    public void Acelerar(float tiempo, float valor)
    {
        acelerarList.Add(valor);
        RecalcularRafaga();

        StartCoroutine(RemoverAcelerar(tiempo, valor));
    }

    private IEnumerator RemoverAcelerar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        acelerarList.Remove(valor);
        RecalcularRafaga();
    }

    // ---------------- RALENTIZAR ----------------
    public void Ralentizar(float tiempo, float valor)
    {
        ralentizarList.Add(valor);
        RecalcularRafaga();

        StartCoroutine(RemoverRalentizar(tiempo, valor));
    }

    private IEnumerator RemoverRalentizar(float tiempo, float valor)
    {
        yield return new WaitForSeconds(tiempo);
        ralentizarList.Remove(valor);
        RecalcularRafaga();
    }

    // ---------------- RECÁLCULO ----------------
    private void RecalcularRafaga()
    {
        float baseRafaga = stats.Rafaga;

        // Aceleraciones acumuladas (sumamos porcentajes)
        float bonus = 0f;
        foreach (var a in acelerarList)
            bonus += a;

        // Ralentización más fuerte
        float slow = ralentizarList.Count > 0 ? Mathf.Max(ralentizarList.ToArray()) : 0f;

        // Fórmula: rafaga = base + (base * (bonus - slow))
        rafaga = baseRafaga + (baseRafaga * (bonus - slow));

        // Límite inferior
        rafaga = Mathf.Max(rafaga, 0.65f);
    }
}
