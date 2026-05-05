using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private TipoNivel tipoProximo;

    public TipoNivel TipoNivel => tipoProximo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.LoadScene(tipoProximo);
            AudioManager.Instance.PlaySFX(GameAssets.i.teleport);
        }
    }

    public void SetProximoNivel(TipoNivel tipo)
    {
        tipoProximo = tipo;
    }
}
public enum TipoNivel
{
    Base, Pelea, Jefe, Descanso
}
