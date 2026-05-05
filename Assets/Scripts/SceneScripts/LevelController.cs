using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    Portal portal;
    Spawner[] spawner;
    private int cantSpawners = 0;

    private void Start()
    {
        portal = GetComponentInChildren<Portal>();
        portal.gameObject.SetActive(false);
        spawner = GetComponentsInChildren<Spawner>();
        cantSpawners=spawner.Length;
        foreach(Spawner s in spawner)
        {
            s.SpawnTerminado += RestarSpawn;
            s.SpawnEnemies(GenerarNumero(LevelManager.Nivel));
        }
    }
    private int GenerarNumero(int n)
    {
        if (n == 1)
            return 1;

        if (n == 2)
            return Random.Range(1, 3); // 1 o 2

        // Para n >= 3
        return Random.value < 0.5f ? n - 1 : n + 1;
    }
    public void RestarSpawn()
    {
        cantSpawners--;
        if (cantSpawners == 0) { FinNivel(); };
    }

    public void FinNivel()
    {
        JugadorSM.Instancia.FinNivel();
        switch (portal.TipoNivel)
        {
            case TipoNivel.Pelea:
                if(LevelManager.Nivel < 4)
                {
                    portal.SetProximoNivel(TipoNivel.Pelea);
                }
                else
                { 
                    portal.SetProximoNivel(TipoNivel.Descanso);
                }
                StartCoroutine(InicializarEpifaniasConDelay());
                break;
            case TipoNivel.Base:
                portal.SetProximoNivel(TipoNivel.Pelea);
                break;
            case TipoNivel.Jefe:
                portal.SetProximoNivel(TipoNivel.Base);
                break;
            case TipoNivel.Descanso:
                portal.SetProximoNivel(TipoNivel.Jefe);
                break;

        }
        portal.gameObject.SetActive(true);
    }

    private IEnumerator InicializarEpifaniasConDelay()
    {

        yield return new WaitForSeconds(0.15f);
        AudioManager.Instance.PlaySFX(GameAssets.i.epifaniasFinNivel);
        yield return new WaitForSeconds(0.35f);
        Pausar.PausarJuego();
        ControlPantallaEpifanias.Instancia.Inicializar();
    }
}
