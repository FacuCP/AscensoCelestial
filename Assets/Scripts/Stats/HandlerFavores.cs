using System;
using Unity.VisualScripting;
using UnityEngine;

public class HandlerFavores : MonoBehaviour
{
    [SerializeField] HandlerPoder handlerPoder;
    [SerializeField] HandlerAtaque handlerAtaque;
    [SerializeField] private FavorBase favor;
    public FavorBase Favor => favor;
    private float carga = 0;

    public event Action<int> CambioCargaActual, CambioCargaMaxima;
    public event Action<string> CambioFavor;
    void Start()
    {
        if (favor) { 
            favor.Setup(this.gameObject.GetComponentInParent<JugadorSM>());
            carga = favor.Coste;
        };
    }
    public void AgregarFavorPrefab(GameObject favorPrefab)
    {
        if (favorPrefab == null)
        {
            Debug.LogWarning("AgregarFavorPrefab recibió null");
            return;
        }

        // Instanciar
        GameObject instancia = Instantiate(favorPrefab, transform);
        instancia.SetActive(true);

        // Obtener ForjaBase
        FavorBase fav = instancia.GetComponent<FavorBase>();
        if (fav == null)
        {
            Debug.LogError($"El prefab {fav.name} no tiene un componente FavorBase");
            Destroy(instancia);
            return;
        }
        // Agregarla a la lista
        AgregarFavor(fav);
    }
    public void AgregarFavor(FavorBase favor)
    {
        if (this.favor == null)
        {
            this.favor = favor;
            favor.Setup(this.gameObject.GetComponentInParent<JugadorSM>());
        }
        else if (this.favor.GetNombre() == favor.GetNombre())
        {
            this.favor.AumentarNivel();
        }
        carga = this.favor.Coste;
        Conectar();
    }

    public void Conectar()
    {
        CambioFavor?.Invoke($"{favor.GetNombre()} Nvl: {favor.Nivel}");
        CambioCargaMaxima?.Invoke((int)carga);
        CambioCargaActual?.Invoke((int)this.favor.Coste);
    }

    public void Despojar()
    {
        favor= null;
        CambioFavor?.Invoke("");
        CambioCargaMaxima?.Invoke((int)carga);
        CambioCargaActual?.Invoke(0);
    }
    private void OnEnable()
    {
        handlerPoder.HizoDmg += CargarFavor;
        handlerAtaque.HizoDmg += CargarFavor;
        if(favor)favor.Setup(this.gameObject.GetComponentInParent<JugadorSM>());
    }
    private void OnDisable()
    {
        handlerPoder.HizoDmg -= CargarFavor;
        handlerAtaque.HizoDmg -= CargarFavor;
    }

    public void Castear(Vector3 punto)
    {
        if (!favor) return;
        if (!Mathf.Approximately(carga, favor.Coste)) { return; }
        carga = 0;
        CambioCargaActual?.Invoke((int)carga);
        favor.Lanzar(punto);
    }
    private void CargarFavor(float valor)
    {
        if (this.favor == null) { return; }
        carga += valor;
        if (carga >= favor.Coste)
        {
            carga = favor.Coste;
        }
        CambioCargaActual?.Invoke((int)carga);
    }
}
