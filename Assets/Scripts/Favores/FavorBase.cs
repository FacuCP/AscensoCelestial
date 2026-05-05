using UnityEngine;

public abstract class FavorBase : MonoBehaviour, IDescripcion
{
    [SerializeField] private int costeBase = 100;
    protected int coste;
    protected StateMachine padre; 
    protected int nivel = 1;

    public int Nivel => nivel;
    public int Coste => coste;
    public virtual void Lanzar(Vector3 punto)
    {
        
    }
    public virtual void Setup(StateMachine padre)
    {
        this.padre = padre;
        this.coste = costeBase;
    }
    public void AumentarNivel()
    {
        nivel = nivel + 1;
        if (nivel > 6) { return; }
        coste = Mathf.Max(1, Mathf.RoundToInt(costeBase * (1 - 0.1f * (nivel - 1))));
    }

    public abstract string GetDescripcion();
    public abstract string GetNombre();
}
