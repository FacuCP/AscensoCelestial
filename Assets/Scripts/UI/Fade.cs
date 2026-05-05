using TMPro;
using UnityEngine;

public class Fade : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI texto;
    [SerializeField] private float duracion = 2f;

    void Update()
    {
        Color c = texto.color;
        c.a = Mathf.PingPong(Time.time / duracion, 1f);
        texto.color = c;
    }
}
