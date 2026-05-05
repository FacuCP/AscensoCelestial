using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;

public enum TipoPopUp
{
    Dmg,
    Cura,
    Crit
}
public class PopUpVida : MonoBehaviour
{
    public static PopUpVida Crear(Vector3 posicion, int valor, TipoPopUp tipo)
    {
        Transform popupVidaTransform = Instantiate(GameAssets.i.popUpVida, posicion, Quaternion.identity);

        PopUpVida popUpVida = popupVidaTransform.GetComponent<PopUpVida>();
        popUpVida.Setup(valor, tipo);

        return popUpVida;
    }

    private static int orden;

    private const float TIMER_DESAPARICION_MAXIMO = 1f;
   
    private TextMeshPro textMesh;
    private Color textColor;
    private float desaparecerTimer;
    private Dictionary<TipoPopUp, (float fontSize, Color32 colorTexto, Color32 colorOutline)> stats = new Dictionary<TipoPopUp, (float, Color32, Color32)>()
    {
        { TipoPopUp.Cura, (4f, new Color32(80,220,80,255), new Color32(20,130,20, 255)) },
        { TipoPopUp.Dmg,  (4f, new Color32(200,85,45,255),new Color32(120,25,10, 255) )},
        { TipoPopUp.Crit, (5.5f,new Color32(255,0,0, 255),new Color32(160,0,0, 255)) }
    };

    private Vector3 moveVector;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int valor, TipoPopUp tipo)
    {
        string text = tipo != TipoPopUp.Crit ? valor.ToString() : valor.ToString() + '!';
        textMesh.SetText(text);
        textMesh.fontSize = stats[tipo].fontSize;
        textMesh.faceColor = stats[tipo].colorTexto;
        textMesh.outlineColor = stats[tipo].colorOutline;
        textColor = textMesh.faceColor;

        orden++;
        textMesh.sortingOrder = orden;

        desaparecerTimer = TIMER_DESAPARICION_MAXIMO;

        moveVector = new Vector3(.7f, 1) * 15f;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f  * Time.deltaTime;

        if(desaparecerTimer > TIMER_DESAPARICION_MAXIMO * .5f) {
            float subirEscala = 1f;
            transform.localScale += Vector3.one * subirEscala * Time.deltaTime;
        } else {
            float bajarEscala = 1.5f;
            transform.localScale -= Vector3.one * bajarEscala * Time.deltaTime;
        }

            desaparecerTimer -= Time.deltaTime;
        if (desaparecerTimer< 0)
        {
            float velDesaparicion = 3f; 
            textColor.a -= velDesaparicion * Time.deltaTime;
            textMesh.faceColor = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
