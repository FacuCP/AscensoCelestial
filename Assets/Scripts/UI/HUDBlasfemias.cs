using TMPro;
using UnityEngine;

public class HUDBlasfemias : MonoBehaviour
{
    private HandleBlasfemias handler;
    private TextMeshProUGUI texto;
    void Start()
    {
        handler = JugadorSM.Instancia.GetComponentInChildren<HandleBlasfemias>();
        texto = GetComponentInChildren<TextMeshProUGUI>();
        handler.CambioBlasfemia += SetTexto;
        gameObject.SetActive(false);
    }

    private void SetTexto(string valor)
    {
        gameObject.SetActive(valor!="");
        texto.text = valor;
    }

}
