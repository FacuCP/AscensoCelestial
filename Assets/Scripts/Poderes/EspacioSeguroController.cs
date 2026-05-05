using System.Collections.Generic;
using UnityEngine;

public class EspacioSeguroController : CuerpoPoder
{
    
    private EspacioSeguro prop; 
    private Dictionary<Collider, float> ultimoTick = new Dictionary<Collider, float>();
    private float intervaloTick = 0.25f;
    public override void Setup(Vector3 direccionInicial, PoderBase propietario, LayerMask capaObjetivo)
    {
        base.Setup(direccionInicial, propietario, capaObjetivo);
        gameObject.layer = 12;
        prop = (EspacioSeguro)propietario;
        float t = Mathf.Clamp01(propietario.Handler.Aura / prop.AuraMax);
        float curva = Mathf.Log10(1 + 9 * t); // rápido al inicio, se aplasta al final
        float factor = Mathf.Lerp(prop.EscalaMin, prop.EscalaMax, curva);
        transform.localScale = Vector3.one * factor; // arranca en 1 y escala según aura
        transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
    }

    private void OnTriggerStay(Collider other)
    {
        int capa = other.gameObject.layer;
        // Verificar si podemos aplicar el tick
        float ahora = Time.time;
        if (ultimoTick.TryGetValue(other, out float ultimo))
        {
            if (ahora - ultimo < intervaloTick) return; // aún no pasó el intervalo
        }
        ultimoTick[other] = ahora; // actualizar tiempo del último tick

        // Si está en la capa objetivo → impacto normal
        if (((1 << capa) & capaObjetivo.value) != 0)
        {
            PoderPropietario.NotificarImpacto(other, this, false);
            return;
        }

        // Determinar la capa opuesta
        int capaOpuesta = (capaObjetivo.value == (1 << 7)) ? 8 : 7;

        // Si es la capa opuesta → impacto especial
        if (capa == capaOpuesta)
        {
            PoderPropietario.NotificarImpacto(other, this, true);
        }
    }

    public override void Reflejar(){}
}
