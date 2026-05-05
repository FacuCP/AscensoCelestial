using UnityEngine;

[System.Serializable]
public class DialogoEntry  // Cambia nombre para claridad
{
    [SerializeField] public Personaje personaje;
    [SerializeField] public string texto;
    // Opcional: Sprite retrato, audio, etc.
}

public enum Personaje
{
    DIOS, LUCIFER, ZADQUIEL, MEL, JELIEL
}

public static class PersonajeExtensions
{
    public static string GetNombre(this Personaje personaje)
    {
        switch (personaje)
        {
            case Personaje.DIOS:
                return "Dios";
            case Personaje.LUCIFER:
                return "Lucifer";
            case Personaje.ZADQUIEL:
                return "Zadquiel";
            case Personaje.MEL:
                return "Mel";
            case Personaje.JELIEL:
                return "Jeliel";
            default:
                return personaje.ToString();
        }
    }

    public static Sprite GetImagen(this Personaje personaje)
    {
        switch (personaje)
        {
            case Personaje.DIOS:
                return Resources.Load<Sprite>("Dialogos/Fotos Personajes/dios");
            case Personaje.LUCIFER:
                return Resources.Load<Sprite>("Dialogos/Fotos Personajes/lucifer");
            case Personaje.ZADQUIEL:
                return Resources.Load<Sprite>("Dialogos/Fotos Personajes/angel");
            case Personaje.MEL:
                return Resources.Load<Sprite>("Dialogos/Fotos Personajes/mel");
            case Personaje.JELIEL:
                return Resources.Load<Sprite>("Dialogos/Fotos Personajes/dios");
            default:
                return null;
        }
    }
}