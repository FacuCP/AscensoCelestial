using System;
using System.Collections.Generic;
using UnityEngine;

public class HandleBlasfemias : MonoBehaviour
{

    [SerializeField] private BlasfemiaBase blasfemia;

    public BlasfemiaBase Blasfemia => blasfemia;
    void Start()
    {
        if(blasfemia)blasfemia.Setup(GetComponent<Estadisticas>());
    }
    public void AgregarBlasfemiaPrefab(GameObject blasfemiaPrefab)
    {
        if (blasfemiaPrefab == null)
        {
            Debug.LogWarning("AgregarBlasfemiaPrefab recibió null");
            return;
        }

        // Instanciar
        GameObject instancia = Instantiate(blasfemiaPrefab, transform);
        instancia.SetActive(true);

        // Obtener ForjaBase
        BlasfemiaBase blasfem = instancia.GetComponent<BlasfemiaBase>();
        if (blasfem == null)
        {
            Debug.LogError($"El prefab {blasfem.name} no tiene un componente BlasfemiaBase");
            Destroy(instancia);
            return;
        }
        // Agregarla a la lista
        AgregarBlasfemia(blasfem);
    }

    public event Action<string> CambioBlasfemia;
    public void AgregarBlasfemia(BlasfemiaBase blasfemia) {
        if (this.blasfemia == null)
        {
            blasfemia.Setup(GetComponent<Estadisticas>());
            this.blasfemia = blasfemia;
        }
        else if (this.blasfemia.GetNombre() == blasfemia.GetNombre())
        {
            this.blasfemia.AumentarNivel();
        }
        CambioBlasfemia?.Invoke($"{this.blasfemia.GetNombre()} Nvl: {this.blasfemia.Nivel}");
    }
    public void Despojar()
    {
        blasfemia = null;
        CambioBlasfemia?.Invoke("");
    }
}
