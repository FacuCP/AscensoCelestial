using System.Collections;
using UnityEngine;

public class Bloqueo : ForjaBase
{
    [SerializeField] float esperaBase = 5;
    [SerializeField] float esperaNivel = 0.5f;
    private bool listo = true;
    public override void Lanzar(GameObject objetivo, LayerMask capaObjetivo, bool aplica, bool critico, float tiempo)
    {
        if (listo)
        {
            listo = false;

            CuerpoPoder target = objetivo.GetComponent<CuerpoPoder>();

            if(target != null)target.Reflejar();

            StartCoroutine(Cooldown());
        }
    }
    public override string GetDescripcion() { return "Los ataques pueden interceptar proyectiles, bloqueándolos y devolviéndolos hacia su origen. Esta forja tiene un tiempo de recarga de 5 segundos, el cual se reduce con cada nivel."; }
    public override string GetNombre() { return "Bloqueo"; }
    private IEnumerator Cooldown()
    {
        float tiempo = Mathf.Max(1f, esperaBase - esperaNivel * Nivel);
        yield return new WaitForSeconds(tiempo);

        listo = true;
    }
}

