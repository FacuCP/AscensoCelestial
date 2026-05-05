using UnityEngine;

[CreateAssetMenu(fileName = "Epifania", menuName = "Scriptable Objects/Epifania")]
public class Epifania : ScriptableObject
{
    public int vida, velocidad, aliento, aura, fuerza, suerte, prisa, alivio;
    public float rafaga;
    public TipoEpifania tipo;
    public Habilidad habilidad;

    public override string ToString()
    {
        string hab = "Sin Habilidad";
        if (habilidad != null) {
            hab = habilidad.tipo + "\n\n" + habilidad.name + "\n\n" + habilidad.GetDescripcion();
        }
        string texto = "Epifania " + tipo.ToTexto() + "\n" + hab;
        return texto;
    }
}
public enum TipoEpifania { 
    CELESTIAL, MUNDANA, EPICA, DIVINA, INFERNAL, PODER, FORJA
}
public static class TipoEpifaniaExtensions
{
    public static string ToTexto(this TipoEpifania tipo)
    {
        return tipo switch
        {
            TipoEpifania.CELESTIAL => "CELESTIAL",
            TipoEpifania.MUNDANA => "MUNDANA",
            TipoEpifania.EPICA => "EPICA",
            TipoEpifania.DIVINA => "DIVINA",
            TipoEpifania.INFERNAL => "INFERNAL",
            _ => ""
        };
    }
}
