using TMPro;
using UnityEngine;

public class AreaLucifer : MonoBehaviour
{
    private static AreaLucifer instance;
    public static AreaLucifer Instancia => instance;


    private static bool playerInArea = false;
    [SerializeField] Canvas canvas;

    private TextMeshProUGUI texto;
    private float distanciaMin = 1.5f; // alpha = 1
    private float distanciaMax = 7f;

    public static bool PlayerInArea => playerInArea;

    Conversacion[] dialogos;
    private Conversacion convActual;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        var originales = Resources.LoadAll<Conversacion>("Dialogos/Dialogos/Lucifer");

        dialogos = new Conversacion[originales.Length];

        for (int i = 0; i < originales.Length; i++)
        {
            dialogos[i] = Instantiate(originales[i]);
        }
        texto = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void IniciarDialogo()
    {
        convActual = Conversacion.ObtenerConversacion(dialogos);
        DialogoController.Instancia.InicializarDialogo(convActual);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
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
