using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversacion", menuName = "Scriptable Objects/Conversacion")]
public class Conversacion : ScriptableObject
{
    [SerializeField] private string _id = System.Guid.NewGuid().ToString(); // ID único auto-generado
    [SerializeField] private int _prioridad = 0; // 0=normal, >0 alta, <0 baja
    [SerializeField] private bool _leida = false; // Estado persistente
    [SerializeField] private bool unica;

    [SerializeField] public List<DialogoEntry> dialogos;

    [Header("Condiciones")]
    [SerializeField] private bool primeraMuerte, primerEnfrentamientoMel, primerVictoriaMel, ultimaVictoriaMel;


    // Propiedades públicas
    public string ID => _id;
    public int Prioridad => _prioridad;
    public bool Unica => unica;

    public bool Leida
    {
        get => _leida;
        set
        {
            _leida = value;
            GuardarEstado(); // Auto-guarda al cambiar
        }
    }

    // Serializa solo datos persistentes (no el array base)
    [Serializable]
    private class ConversacionData
    {
        public string id;
        public int prioridad;
        public bool leida;
    }

    public void GuardarEstado()
    {
        var data = new ConversacionData
        {
            id = _id,
            prioridad = _prioridad,
            leida = _leida,
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Conv_" + _id, json);
        PlayerPrefs.Save();
    }

    public void CargarEstado()
    {
        if (PlayerPrefs.HasKey("Conv_" + _id))
        {
            string json = PlayerPrefs.GetString("Conv_" + _id);
            var data = JsonUtility.FromJson<ConversacionData>(json);
            _leida = data.leida;
            // _prioridad = data.prioridad; // Opcional, ya que prioridad suele ser estática
        }
    }

    public void Leer()
    {
        Leida = true;
    }

    public static Conversacion ObtenerConversacion(Conversacion[] conversaciones)
    {
        if (conversaciones == null || conversaciones.Length == 0)
            return null;

        // 1. Actualizar estado de todas
        foreach (var conv in conversaciones)
        {
            conv.CargarEstado();
            conv.Actualizar();
        }
        // 2. Filtrar conversaciones válidas
        List<Conversacion> candidatas = new List<Conversacion>();

        foreach (var conv in conversaciones)
        {
            if(conv.EsElegible()) candidatas.Add(conv);
        }

        if (candidatas.Count == 0)
            return conversaciones[0];

        // 3. Ordenar:
        // - Primero no leídas
        // - Luego mayor prioridad
        candidatas.Sort((a, b) =>
        {
            int leidaCompare = a.Leida.CompareTo(b.Leida); // false (0) antes que true (1)
            if (leidaCompare != 0)
                return leidaCompare;

            return b.Prioridad.CompareTo(a.Prioridad); // mayor prioridad primero
        });

        // 4. Tomar el mejor grupo (misma leída + misma prioridad)
        bool mejorLeida = candidatas[0].Leida;
        int mejorPrioridad = candidatas[0].Prioridad;

        List<Conversacion> mejores = new List<Conversacion>();

        foreach (var conv in candidatas)
        {
            if (conv.Leida == mejorLeida && conv.Prioridad == mejorPrioridad)
                mejores.Add(conv);
            else
                break; // ya no pertenece al mejor grupo
        }

        // 5. Elegir una aleatoria del mejor grupo
        int index = UnityEngine.Random.Range(0, mejores.Count);
        return mejores[index];
    }

    private void Actualizar()
    {
        if(_leida)return;
        if (TrackerHistoria.Instancia.primeraMuerte && primeraMuerte && _prioridad == 0)
        {
            _prioridad = 10;
        }
        if (TrackerHistoria.Instancia.primerEnfrentamientoMel && primerEnfrentamientoMel && _prioridad == 0)
        {
            _prioridad = 10;
        }
        if (TrackerHistoria.Instancia.cantidadVictorias == 1 && primerVictoriaMel && _prioridad == 0)
        {
            _prioridad = 10;
        }
        if (TrackerHistoria.Instancia.cantidadVictorias == (TrackerHistoria.cantidadVictoriasJuego - 1) && ultimaVictoriaMel && _prioridad == 0)
        {
            _prioridad = 10;
        }
        GuardarEstado();
    }

    private bool EsElegible()
    {
        bool value = true;
        // Si es única y ya fue leída, se descarta
        if (this.Leida && this.Unica)
            value = false;

        if (!TrackerHistoria.Instancia.primeraMuerte && this.primeraMuerte)
        {
            value = false;
        }
        if (!TrackerHistoria.Instancia.primerEnfrentamientoMel && this.primerEnfrentamientoMel)
        {
            value =  false;
        }
        if (TrackerHistoria.Instancia.cantidadVictorias < 1 && this.primerVictoriaMel)
        {
            value = false;
        }
        if (TrackerHistoria.Instancia.cantidadVictorias < (TrackerHistoria.cantidadVictoriasJuego - 1) && this.ultimaVictoriaMel)
        {
            value = false;
        }

        return value;
    }
}