using UnityEngine;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private GameObject reset;

    public void Start()
    {
        reset.SetActive(false); 
        AudioManager.Instance.PlayMusica(GameAssets.i.musicaMenu);
    }
    public void Jugar()
    {
        LevelManager.Instance.LoadScene(TipoNivel.Base);
    }

    public void Salir()
    {
        Application.Quit();
    }


    public void Resetear()
    {
        reset.SetActive(true);
    }
    public void CerrarReset()
    {
        reset.SetActive(false);
    }

    public void ConfirmarReset()
    {
        SaveLoadManagerJson.Instancia.ResetearTodo();
        CerrarReset();
        Application.Quit();
    }
}
