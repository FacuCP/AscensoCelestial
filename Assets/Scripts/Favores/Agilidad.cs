using UnityEngine;

public class Agilidad : FavorBase
{
    [SerializeField] private float rango;
    public override void Lanzar(Vector3 punto)
    {
        if (padre != null)
        {
            Vector3 direccion = (punto - padre.transform.position).normalized;
        direccion.y = 0f;
            padre.transform.position += direccion * rango;
        }
    }

    public override string GetDescripcion()
    {
        return "Desplaza al jugador en la dirección a la que esté apuntando.";
    }
    public override string GetNombre() { return "Agilidad"; }
}
