using UnityEngine;

public class TrackerHistoria : MonoBehaviour
{
    [SerializeField]public bool primeraMuerte /*{ get; private set; }*/ = false;
    [SerializeField] public bool primerEnfrentamientoMel /*{ get; private set; }*/ = false;
    [SerializeField] public int cantidadVictorias /*{ get; private set; }*/ = 0;

    [SerializeField] public const int cantidadVictoriasJuego = 3;

    public static TrackerHistoria Instancia { get; private set; }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

    }

    public void Start()
    {
        Cargar();
    }

    // ===== API PÚBLICA (modifica + guarda) =====

    public void PrimeraMuerte()
    {
        if (primeraMuerte) return;

        primeraMuerte = true;
        Guardar();
    }

    public void PrimerEnfrentamientoMel()
    {
        if (primerEnfrentamientoMel) return;

        primerEnfrentamientoMel = true;
        Guardar();
    }

    public void Ganar()
    {
        cantidadVictorias++;
        Guardar();
    }

    // ===== PERSISTENCIA =====

    private void Guardar()
    {
        HistoriaData data = new HistoriaData
        {
            primeraMuerte = primeraMuerte,
            primerEnfrentamientoMel = primerEnfrentamientoMel,
            cantidadVictorias = cantidadVictorias
        };

        SaveLoadManagerJson.Instancia.GuardarHistoria(data);
    }

    public void Cargar()
    {
        HistoriaData data = SaveLoadManagerJson.Instancia.CargarHistoria();

        if (data == null)
            return;

        primeraMuerte = data.primeraMuerte;
        primerEnfrentamientoMel = data.primerEnfrentamientoMel;
        cantidadVictorias = data.cantidadVictorias;

    }
}
