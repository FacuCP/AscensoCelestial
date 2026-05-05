using System;
using UnityEngine;

public abstract class ForjaBase : MonoBehaviour, IDescripcion
{
    [SerializeField] public string nombre;
    [SerializeField] private int nivel = 1;
    private bool disponible = true;
    protected float inicioAtaque = 0;
    protected HandlerAtaque handler;
    public virtual void SetHandler(HandlerAtaque handler) { this.handler = handler; }

    public HandlerAtaque Handler => handler;
    public int Nivel => nivel;
    public bool Disponible => disponible;

    public virtual void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        /*Vector3 direccion = (punto - origen).normalized;
        direccion.y = 0f;
        if (prefabProyectil != null && disponible)
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            rotacion *= Quaternion.Euler(0, 90, 0);
            GameObject proyectilGO = Object.Instantiate(prefabProyectil, origen, rotacion);
            CuerpoPoder cuerpo = proyectilGO.GetComponent<CuerpoPoder>();

            cuerpo.Setup(direccion, this, velocidad, capaObjetivo);
            IniciarEspera();
        }*/
    }

    public abstract string GetDescripcion();
    public abstract string GetNombre();

    public void SubirNivel()
    {
        nivel++;
    }

    public override bool Equals(object obj)
    {
        return obj is ForjaBase @base &&
               nombre == @base.nombre;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), nombre);
    }
}
