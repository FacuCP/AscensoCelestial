using UnityEngine;
using UnityEngine.UIElements;

public class Persistente : MonoBehaviour
{
    void Awake()
    {
        // Buscar todos los objetos con MI MISMO TAG
        GameObject[] objs = GameObject.FindGameObjectsWithTag(gameObject.tag);

        // Si hay más de uno, destruir el duplicado
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
