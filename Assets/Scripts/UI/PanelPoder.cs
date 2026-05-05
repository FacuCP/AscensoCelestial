using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PanelPoder : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI nombre, seleccion, coste,nivel;
    [SerializeField] Image sprite, circuloCarga, gotaAliento;

    [SerializeField] private Color colorNormal, colorSinAliento;

    [SerializeField] private float escalaSegundoPlano = 0.5f;

   // private HandlerPoder handler;
    private PoderBase poder;

    public PoderBase Poder => poder;

    private void Start()
    {
        gotaAliento.gameObject.SetActive(false);
    }

    public void SetPoder(PoderBase p)
    {
        poder = p;
        nombre.text = p?.GetNombre();
        coste.text = "Coste: " + (p?.Coste * (1 - JugadorSM.Instancia.Stats.Alivio / 100)).ToString();
        nivel.text = "Nivel: " + p?.Nivel.ToString();
        sprite.sprite = p?.Habilidad?.icono;
        if (poder == null) nombre.text = "";
    }
    public void SetPoder(PoderBase p, int numero)
    {
        SetPoder(p);
        seleccion.text = numero.ToString();
    }

    private void Update()
    {
        if (!JugadorSM.Instancia.poder.TieneAlientoPara(poder))
        {
            circuloCarga.color = colorSinAliento;
            circuloCarga.fillAmount = 1;
            gotaAliento.gameObject.SetActive(true);
        }
        else
        {
            circuloCarga.color = colorNormal;
            gotaAliento.gameObject.SetActive(false);
            circuloCarga.fillAmount = poder.TiempoRestante / poder.TiempoEspera();
        }
    }

    public void SegundoPlano()
    {
        transform.localScale = new Vector3(escalaSegundoPlano, escalaSegundoPlano, escalaSegundoPlano);
        coste.gameObject.SetActive(false);
    }

    public void PrimerPlano()
    {
        transform.localScale = Vector3.one;
        coste.gameObject.SetActive(true);
    }
    
}
