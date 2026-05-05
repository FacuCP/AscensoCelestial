using System;
using UnityEngine;

public class HandlerHabDmg : MonoBehaviour
{

    public event Action<float> HizoDmg;

    public void Golpeo(float dmg)
    {
        HizoDmg?.Invoke(dmg);
    }
}
