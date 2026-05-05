using UnityEngine;

[CreateAssetMenu(fileName = "Habilidad", menuName = "Scriptable Objects/Habilidad")]
public class Habilidad : ScriptableObject
{
    public TipoHabilidad tipo;
    public GameObject prefab;
    public string descripcion;
    public Sprite icono;

    public string GetDescripcion()
    {
        if (prefab == null) return "Sin habilidad";

        // Buscar *cualquier* componente del prefab que implemente IDescripcion
        IDescripcion descripcionScript = prefab.GetComponent<IDescripcion>();
        if (descripcionScript != null)
            return descripcionScript.GetDescripcion();

        return "Sin descripción";
    }

    public string GetNombre()
    {
        return name;
    }
    public string GetCoste()
    {
        if (prefab == null) return "";

        switch (tipo)
        {
            case TipoHabilidad.PODER:
                {
                    PoderBase poder = prefab.GetComponent<PoderBase>();
                    if (poder != null)
                        return poder.Coste.ToString();
                    break;
                }

            case TipoHabilidad.FAVOR:
                {
                    FavorBase favor = prefab.GetComponent<FavorBase>();
                    if (favor != null)
                        return favor.Coste.ToString();
                    break;
                }
        }

        return "";
    }
    public string GetEspera()
    {
        if (prefab == null) return "";

        if (tipo == TipoHabilidad.PODER)
        {
            PoderBase poder = prefab.GetComponent<PoderBase>();
            if (poder != null)
                return poder.RecargaBase.ToString("0.##");
        }

        return "";
    }

}

public enum TipoHabilidad { FORJA, PODER, BLASFEMIA, FAVOR}

public static class TipoHabilidadExtensions
{
    public static string ToTexto(this TipoHabilidad tipo)
    {
        return tipo switch
        {
            TipoHabilidad.FORJA => "FORJA",
            TipoHabilidad.PODER => "PODER",
            TipoHabilidad.BLASFEMIA => "BLASFEMIA",
            TipoHabilidad.FAVOR => "FAVOR",
            _ => ""
        };
    }
}