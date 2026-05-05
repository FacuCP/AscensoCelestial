using UnityEngine;

public class Pausar : MonoBehaviour
{

    private static Pausar _i;

    public static Pausar i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<Pausar>("Pausar"));
            return _i;
        }
    }
    
    
    private static bool pausado = false;
    private static bool detenido = false;

    public static bool Pausado => pausado;
    public static bool Detenido => detenido;


    public static void PausarJuego()
    {
        Time.timeScale = 0f;
        pausado = true;
    }

    public static void DespausarJuego()
    {
        Time.timeScale = 1f;
        pausado = false;
    }

    public static void Detener()
    {
        detenido = true;
    }
    public static void Continuar()
    {
        detenido = false;
    }

}
