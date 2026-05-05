using UnityEngine;
public class AnimatorProxy : MonoBehaviour
{
    public SupernovaController padre; // arrastralo en el Inspector

    public void Explotar() {
        if (padre != null)
            padre.Explotar();
    }
    public void FinExplosion()
    {
        if (padre != null)
            padre.FinExplosion(); // llama al método real
    }
}
