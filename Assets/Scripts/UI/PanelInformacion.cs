using System.Runtime.CompilerServices;
using UnityEngine;

public class PanelInformacion : MonoBehaviour
{
    [SerializeField] private GameObject panelComoJugar, panelControles, panelPoderes, panelForjas, panelFavores, panelBlasfemias, panelCombate;

    public void Activar(VentanaActiva ventana)
    {
        gameObject.SetActive(true);
        MostrarVentana(ventana);
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    private void MostrarVentana(VentanaActiva ventana)
    {
        // Desactivar todos
        panelComoJugar.SetActive(false);
        panelControles.SetActive(false);
        panelPoderes.SetActive(false);
        panelForjas.SetActive(false);
        panelFavores.SetActive(false);
        panelCombate.SetActive(false);
        panelBlasfemias.SetActive(false);
        // Activar el correspondiente
        switch (ventana)
        {
            case VentanaActiva.ComoJugar:
                panelComoJugar.SetActive(true);
                break;
            case VentanaActiva.Controles:
                panelControles.SetActive(true);
                break;
            case VentanaActiva.Poderes:
                panelPoderes.SetActive(true);
                break;
            case VentanaActiva.Forjas:
                panelForjas.SetActive(true);
                break;
            case VentanaActiva.Favores:
                panelFavores.SetActive(true);
                break;
            case VentanaActiva.Blasfemias:
                panelBlasfemias.SetActive(true);
                break;
            case VentanaActiva.Combate:
                panelCombate.SetActive(true);
                break;
        }
    }

    public void AbrirComoJugar() => MostrarVentana(VentanaActiva.ComoJugar);
    public void AbrirControles() => MostrarVentana(VentanaActiva.Controles);
    public void AbrirPoderes() => MostrarVentana(VentanaActiva.Poderes);
    public void AbrirForjas() => MostrarVentana(VentanaActiva.Forjas);
    public void AbrirFavores() => MostrarVentana(VentanaActiva.Favores);
    public void AbrirBlasfemias() => MostrarVentana(VentanaActiva.Blasfemias);

    public void AbrirCombate() => MostrarVentana(VentanaActiva.Combate);
}

public enum VentanaActiva
{
    ComoJugar, Controles, Poderes, Forjas, Favores, Blasfemias,Combate
}
