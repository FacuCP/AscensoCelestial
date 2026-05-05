using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Decreto[] decretos;
    public int esencia;
}

[System.Serializable]
public class HistoriaData
{
    public int cantidadVictorias;
    public bool primeraMuerte;
    public bool primerEnfrentamientoMel;
}
