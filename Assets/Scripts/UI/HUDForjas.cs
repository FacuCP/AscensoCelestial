
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDForjas : MonoBehaviour
{
    TextMeshProUGUI texto, titulo;
    private HandlerAtaque handler;

    private void Awake()
    {

        texto = transform.Find("Texto")?.GetComponent<TextMeshProUGUI>();
        titulo = transform.Find("Titulo")?.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        handler = JugadorSM.Instancia.GetComponentInChildren<HandlerAtaque>();
        handler.CambioForjas += SetTexto;
    }

    private void SetTexto(List<ForjaBase> forjas)
    {
        List<string> nombres = new List<string>();
        foreach (ForjaBase forja in forjas) {
            nombres.Add($"{forja.GetNombre()} Nvl: {forja.Nivel}");
        }
        texto.text = "";
        foreach(string n in nombres)
        {
            texto.text += n+"\n";
        }
    }
}
