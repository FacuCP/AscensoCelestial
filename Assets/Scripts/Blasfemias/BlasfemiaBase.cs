using UnityEngine;

public abstract class BlasfemiaBase : MonoBehaviour, IDescripcion
{
    protected Estadisticas stats;
    protected int nivel = 1;
    public int Nivel => nivel;
    public virtual void Setup(Estadisticas stats)
    {
        this.stats = stats;
    }

    public virtual void AumentarNivel()
    {
        nivel++;
    }

    public abstract string GetDescripcion();
    public abstract string GetNombre();
}
