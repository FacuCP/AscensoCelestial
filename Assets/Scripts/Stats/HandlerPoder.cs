using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandlerPoder : HandlerHabDmg
{
    [SerializeField] Estadisticas stats;
    [SerializeField] HandlerAnimacion animador;

    private float aliento;
    private float aura;
    private float alivio;
    private float prisa;
    private int limitePoderes;
    private float modDmg = 1f;

    private float alientoActual;
    private float tiempoAcumulado = 0f;

    public float Aura => aura;
    public float ModDmg => modDmg;
    public void SetModDmg(float valor) { modDmg = valor; }
    public float Prisa => prisa;

    [SerializeField] private LayerMask capaObjetivo;
    [SerializeField] private LayerMask capaPropia;

    public LayerMask CapaPropia => capaPropia;

    private List<PoderBase> poderes = new List<PoderBase>();

    public List<PoderBase> Poderes => poderes;

    private int indiceActual = 0;

    public event Action<int> CambioAlientoActual;
    public event Action<int> CambioAlientoMaximo;

    public void Start()
    {
        stats.CambioEstadisticas += ActualizarStats;
        ActualizarStats();
        CambioPoder?.Invoke(poderes[0]);
    }
    private void ActualizarStats() { 
        aliento = stats.Aliento;
        aura = stats.Aura;
        alivio = stats.Alivio;
        prisa = stats.Prisa;
        limitePoderes = stats.LimitePoderes;
        alientoActual = aliento;
        Conectar();
    }

    public void Conectar()
    {
        CambioAlientoActual?.Invoke((int)alientoActual);
        CambioAlientoMaximo?.Invoke((int)aliento);
        CambioPoder?.Invoke(poderes[indiceActual]);
    }
    public void AgregarPoderPrefab(GameObject poderPrefab)
    {
        if (poderPrefab == null)
        {
            Debug.LogWarning("AgregarPoderPrefab recibió null");
            return;
        }
        // Instanciar
        GameObject instancia = Instantiate(poderPrefab, transform);
        instancia.SetActive(true);
        // Obtener PoderBase
        PoderBase poder = instancia.GetComponent<PoderBase>();
        if (poder == null)
        {
            Debug.LogError($"El prefab {poderPrefab.name} no tiene componente PoderBase");
            Destroy(instancia);
            return;
        }
        // Informar quién lo maneja
        poder.SetHandler(this);
        // Agregarlo realmente
        AgregarPoder(poder);
    }

    public event Action<PoderBase> CambioPoder;
    public void AgregarPoder(PoderBase nuevoPoder)
    {
        if (nuevoPoder == null) return;
        //if (poderes.Count >= limitePoderes)
        // {
        //Debug.LogWarning("No se pueden agregar más poderes (límite alcanzado)");
        // return;
        //}
        //Debug.Log($"Se quiere agregar {nuevoPoder.nombre}, esta en array? {poderes.Contains(nuevoPoder)}");
        if (!poderes.Contains(nuevoPoder))
        {
            poderes.Add(nuevoPoder);
            indiceActual = poderes.Count - 1;
            CambioPoder?.Invoke(poderes[indiceActual]);
        }
        else
        {
            indiceActual = poderes.IndexOf(nuevoPoder); // usa Equals()
            if (indiceActual != -1)
            {
                poderes[indiceActual].SubirNivel();
            }
            CambioPoder?.Invoke(poderes[indiceActual]);
        }
        //foreach (PoderBase p in poderes) Debug.Log($"en array: {p.nombre} nivel: {p.Nivel} long array:{poderes.Count} {Time.time}");
    }


    public void Reiniciar()
    {
        StopAllCoroutines();
        indiceActual = 0;
        CambioPoder?.Invoke(null);
        poderes.Clear();
        modDmg = 1;
    }
    public void Castear(Vector3 punto)
    {
        if (poderes.Count == 0) return;
        if (CanCast())
        {
            ManageAliento(GetCoste());
            poderes[indiceActual].Lanzar(punto, transform.position, capaObjetivo);
        };
    }

    public bool CanCast() {
        if (poderes.Count == 0) return false;
        
        return poderes[indiceActual].Disponible && TieneAliento();
    }

    public bool TieneAliento()
    {
        if (poderes.Count == 0) return false;

        return alientoActual - GetCoste() >= 0;
    }

    public bool TieneAlientoPara(PoderBase p)
    {
        return alientoActual - p.Coste * (1 - alivio / 100) >= 0;
    }

    private float GetCoste()
    {
        return poderes[indiceActual].Coste * (1 - alivio / 100);
    }
    private void ManageAliento(float coste, bool resta = true)
    {
        if (resta)
        {
            alientoActual -= coste;
            alientoActual = Mathf.Max(alientoActual, 0);
        }
        else
        {
            alientoActual += coste;
            alientoActual = Mathf.Min(alientoActual, aliento);
        }
        CambioAlientoActual?.Invoke((int)alientoActual);
    }
    public void Cambio(int direccion)
    {
        if (poderes.Count == 0){ CambioPoder?.Invoke(null); return; }
        indiceActual = (indiceActual + direccion + poderes.Count) % poderes.Count;
        CambioPoder?.Invoke(poderes[indiceActual]);
    }

    public void Seleccionar(int valor)
    {
        if (poderes.Count < valor) return;
        indiceActual = valor-1;
        CambioPoder?.Invoke(poderes[indiceActual]);
    }

    public void SetIndice(int indice)
    {
        if (poderes.Count == 0) { CambioPoder?.Invoke(null); return; }
        indiceActual = Mathf.Clamp(indice,0,poderes.Count);
        CambioPoder?.Invoke(poderes[indiceActual]);
    }
    public void ModificarAlientoProlongado(float cantidad, float tiempo, bool resta = true)
    {
        StartCoroutine(AplicarAlientoProlongado(cantidad, tiempo, resta));
    }

    private IEnumerator AplicarAlientoProlongado(float cantidad, float tiempo, bool resta)
    {
        while (tiempo > 0)
        {
            ManageAliento(cantidad, resta); 
            yield return new WaitForSeconds(0.5f); 
            tiempo -= 0.5f;
        }
    }
    private void Update()
    {
        tiempoAcumulado += Time.deltaTime;
        if (tiempoAcumulado >= 1f)
        {
            ManageAliento(stats.RegenAliento, false);
            tiempoAcumulado = 0f;
        }
    }
}
