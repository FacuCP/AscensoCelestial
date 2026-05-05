using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PoderBase: MonoBehaviour, IDescripcion
{
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] public string nombre;
    [SerializeField] private float recargaBase = 1f;
    [SerializeField] private float coste = 0f;
    [SerializeField] private int nivel = 1;
    protected bool disponible = true; 
    private float recarga = 0;
    private float tiempoRestante = 0f;
    protected HandlerPoder handler;

    [SerializeField] protected AudioClip clipCast;
    [SerializeField] protected AudioClip clipGolpe;

    [SerializeField] private Habilidad habilidad;
    public Habilidad Habilidad => habilidad;
    public void SetHandler(HandlerPoder handler) {  this.handler = handler; }

    public HandlerPoder Handler => handler;
    public float RecargaBase => recargaBase;
    public float Recarga => recarga;
    public float TiempoRestante => tiempoRestante;
    public float Coste => coste;
    public int Nivel => nivel;
    public bool Disponible => disponible;

    public void Start()
    {
        if (clipCast == null) clipCast = GameAssets.i.castPoder;
        if (clipGolpe == null) clipGolpe = GameAssets.i.golpePoder;
    }

    public virtual void   Lanzar(Vector3 punto, Vector3 origen, LayerMask capaObjetivo, bool rotar = true)
    {
        Vector3 direccion = (punto - origen).normalized;
        direccion.y = 0f;
        if (prefabProyectil != null && disponible)
        {
            Quaternion rotacion = rotar
                    ? Quaternion.LookRotation(direccion) * Quaternion.Euler(0, 90, 0)
                    : Quaternion.identity;
            GameObject proyectilGO = GameObject.Instantiate(prefabProyectil, origen, rotacion);
            CuerpoPoder cuerpo = proyectilGO.GetComponent<CuerpoPoder>();
            int capaIndex = Mathf.RoundToInt(Mathf.Log(handler.CapaPropia.value, 2));
            proyectilGO.layer = capaIndex;
            cuerpo.Setup(direccion, this, capaObjetivo);
            AudioManager.Instance.PlaySFX(clipCast);
            IniciarEspera();
        }
    }

    private void Update()
    {
        if (!disponible && tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
        }
        if (tiempoRestante <= 0f)
        {
            disponible = true;
            tiempoRestante = 0f;
        }
    }

    public float TiempoEspera()
    {
        return recargaBase * (1 - handler.Prisa / 100);
    }

    protected void IniciarEspera()
    {
        disponible = false;
        tiempoRestante = recargaBase * (1 - handler.Prisa / 100);
        recarga = tiempoRestante;
    }

    /// <summary>
    /// Adelanta el tiempo de recargaBase manualmente.
    /// </summary>
    public void Adelantar(float segundos)
    {
        if (!disponible)
        {
            tiempoRestante -= segundos;
            if (tiempoRestante <= 0f)
            {
                disponible = true;
                tiempoRestante = 0f;
            }
        }
    }
    internal void NotificarImpacto(Collider other, CuerpoPoder cuerpo, bool aliado =false)
    {
        OnImpacto(other, cuerpo, aliado);
    }

    // Cada poder implementa cómo calcular su daño base
    protected abstract float CalcularDmgBase();

    // Método común: siempre aplica el ModDmg del handler
    public virtual float GetDmg()
    {
        return Handler.ModDmg * CalcularDmgBase();
    }
    protected virtual void OnImpacto(Collider other, CuerpoPoder cuerpo, bool aliado = false) {
        AudioManager.Instance.PlaySFX(clipGolpe,0.6f);
    }

    public abstract string GetDescripcion();
    public abstract string GetNombre();

    public void SubirNivel()
    {
        nivel++;
    }

    public override bool Equals(object obj)
    {
        return obj is PoderBase @base &&
               nombre == @base.nombre;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), nombre);
    }
}