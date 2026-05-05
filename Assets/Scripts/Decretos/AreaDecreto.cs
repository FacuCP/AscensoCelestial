using TMPro;
using UnityEngine;

public class AreaDecreto : MonoBehaviour
{
    private static bool playerInArea = false;
    private static TipoPantallaDecreto tipoActual = TipoPantallaDecreto.BASICOS;
    [SerializeField] TipoPantallaDecreto tipoAltar;
    [SerializeField] Canvas canvas;

    private TextMeshProUGUI texto;
    private float distanciaMin = 1.5f; // alpha = 1
    private float distanciaMax = 7f;

    public static TipoPantallaDecreto TipoActual     => tipoActual;
    public static bool PlayerInArea => playerInArea;

    private void Awake()
    {
        texto = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
            tipoActual = tipoAltar;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }

    private void Update()
    {
        if (JugadorSM.Instancia == null || texto == null)
            return;

        float distancia = Vector3.Distance(
            JugadorSM.Instancia.transform.position,
            transform.position
        );

        float alpha = Mathf.InverseLerp(distanciaMax, distanciaMin, distancia);

        Color c = texto.color;
        c.a = alpha;
        texto.color = c;
    }
}
