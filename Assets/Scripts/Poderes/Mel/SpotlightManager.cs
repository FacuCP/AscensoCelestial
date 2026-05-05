using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SpotlightManager : MonoBehaviour
{
    [SerializeField] SphereCollider area;
    [SerializeField] GameObject spotlight;

    float distanciaMinimaEntreSpotlights = 9f;
    int intentosMaximosPorSpotlight = 20;

    private List<GameObject> spotlightsActivos = new();
    private Spotlight spotlightActivo;
    public Spotlight SpotlightActivo => spotlightActivo;
    public event Action FinalActo;

    [SerializeField] private int cantidadInicialMoviles = 3;
    [SerializeField] private int cantidadMaximaMoviles = 10;
    [SerializeField] private int incrementoPorOleada = 2;

    private int cantidadActualMoviles;
    private int lucesMovilesVivas;
    private bool cicloMovilesActivo = false;

    public void EmpezarBallet(int cantidad)
    {
        List<Vector3> posicionesUsadas = new();

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 pos = BuscarPosicionValida(posicionesUsadas);
            posicionesUsadas.Add(pos);

            CrearSpotlight(false, false, pos);
        }

        NuevoSpot();
    }

    public void NuevoSpot()
    {
        spotlightActivo = null;

        foreach (var obj in spotlightsActivos)
        {
            if (obj == null) continue;

            Spotlight s = obj.GetComponent<Spotlight>();
            if (!s.EsEspecial)
            {
                spotlightActivo = s;
                break;
            }
        }
        if(spotlightActivo == null) FinalActo?.Invoke();
    }

    public void TerminarSpot()
    {
        if (spotlightActivo == null)
            return;

        GameObject obj = spotlightActivo.gameObject;

        spotlightActivo.Destruir();
        spotlightsActivos.Remove(obj);

        spotlightActivo = null;

        NuevoSpot();
    }

    public void CrearLucesMoviles(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            CrearSpotlight(true, true);
        }
    }

    public void AparecerFuego(Vector3 pos)
    {
        pos = new Vector3(pos.x,0,pos.z);
        GameObject obj = Instantiate(spotlight, pos, Quaternion.identity);
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1.3f,1.3f,1.3f);
        Spotlight s = obj.GetComponent<Spotlight>();
        s.Inicializar(true, false, area,pos);
    }

    private void CrearSpotlight(bool haceDmg, bool seMueve, Vector3 pos)
    {
        GameObject obj = Instantiate(spotlight, pos, Quaternion.identity, transform);

        Spotlight s = obj.GetComponent<Spotlight>();
        s.Inicializar(haceDmg, seMueve, area);

        spotlightsActivos.Add(obj);
    }
    private void CrearSpotlight(bool haceDmg, bool seMueve)
    {
        CrearSpotlight(haceDmg, seMueve, PosicionAleatoriaEnArea());
    }

    private Vector3 PosicionAleatoriaEnArea()
    {
        Vector3 centro = area.transform.position;
        float radio = area.radius * area.transform.localScale.x;

        Vector3 randomPos = centro + Random.insideUnitSphere * radio;
        randomPos.y = centro.y;

        return randomPos;
    }
    private Vector3 BuscarPosicionValida(List<Vector3> posicionesExistentes)
    {
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < intentosMaximosPorSpotlight; i++)
        {
            pos = PosicionAleatoriaEnArea();
            bool valida = true;

            foreach (var existente in posicionesExistentes)
            {
                if (Vector3.Distance(pos, existente) < distanciaMinimaEntreSpotlights)
                {
                    valida = false;
                    break;
                }
            }

            if (valida)
                return pos;
        }

        // Fail-safe: devuelve la última aunque no sea ideal
        return pos;
    }


    public void IniciarCicloLucesMoviles()
    {
        if (cicloMovilesActivo) return;

        cicloMovilesActivo = true;
        cantidadActualMoviles = cantidadInicialMoviles;

        CrearOleadaMovil();
    }

    private void CrearOleadaMovil()
    {
        lucesMovilesVivas = 0;

        for (int i = 0; i < cantidadActualMoviles; i++)
        {
            GameObject obj = Instantiate(spotlight, PosicionAleatoriaEnArea(), Quaternion.identity, transform);

            Spotlight s = obj.GetComponent<Spotlight>();
            s.Inicializar(true, true, area);

            s.OnDestroyed += OnSpotlightMovilDestruido;

            spotlightsActivos.Add(obj);
            lucesMovilesVivas++;
        }
    }

    private void OnSpotlightMovilDestruido(Spotlight s)
    {
        s.OnDestroyed -= OnSpotlightMovilDestruido;

        lucesMovilesVivas--;

        if (lucesMovilesVivas <= 0)
        {
            AvanzarOleada();
        }
    }

    private void AvanzarOleada()
    {
        cantidadActualMoviles = Mathf.Min(
            cantidadActualMoviles + incrementoPorOleada,
            cantidadMaximaMoviles
        );

        CrearOleadaMovil();
    }
}
