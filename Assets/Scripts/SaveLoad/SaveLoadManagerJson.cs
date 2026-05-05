using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManagerJson : MonoBehaviour
{
    private string filePath;
    private string historiaPath;

    private string saveFile = "/savefile.json";
    private string historiaFile ="/historia.json";
    public static SaveLoadManagerJson Instancia { get; private set; }


    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instancia = this;
        filePath = Application.persistentDataPath + saveFile;
        historiaPath = Application.persistentDataPath + historiaFile;
    }

    public void SaveGame(Decreto[] decretos,int esencia)
    {
        SaveData data = new SaveData();
        data.esencia = esencia;
        data.decretos = decretos;
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(filePath, json);

    }

    public SaveData LoadGame()
    {
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);

            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            return null;
        }
    }

    // ===== HISTORIA =====

    public void GuardarHistoria(HistoriaData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(historiaPath, json);
    }

    public HistoriaData CargarHistoria()
    {
        if (!File.Exists(historiaPath))
            return null;

        string json = File.ReadAllText(historiaPath);
        return JsonUtility.FromJson<HistoriaData>(json);
    }


    public void ResetearTodo()
    {
        string saveFilePath = Application.persistentDataPath + saveFile;
        string historiaFilePath = Application.persistentDataPath + historiaFile;
        // ===== BORRAR SAVE PRINCIPAL =====
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            else
            {
                Debug.LogError("Error al borrar save");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al borrar save: " + e.Message);
        }


        // ===== BORRAR HISTORIA =====
        try
        {
            if (File.Exists(historiaFilePath))
            {
                File.Delete(historiaFilePath);
            }
            else
            {
                Debug.LogError("Error al borrar historia");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al borrar historia: " + e.Message);
        }

        // ===== OPCIONAL: LIMPIAR PLAYER PREFS =====
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        TrackerHistoria.Instancia.Cargar();
    }
}
