using UnityEngine;

public class Masoquismo : BlasfemiaBase
{
    private HandlerEstados estados;
    private HandlerVida vida;
    [SerializeField] float radioEmpoderado = 5f;
    [SerializeField] float radioAcelerado = 5f;
    [SerializeField] float radioCura = 5f;
    [SerializeField] float empoderadoNivel = 1f;
    [SerializeField] float aceleradoNivel = 1f;
    [SerializeField] float curaNivel = 1f;
    [SerializeField] float duracion = 5f;
    public override void Setup(Estadisticas stats)
    {
        base.Setup(stats);
        estados = stats.GetComponentInParent<HandlerEstados>();
        vida = stats.GetComponent<HandlerVida>();
        vida.RecibioDmg += Masoquista;
    }

    private void Masoquista(int valor)
    {
        float empoderadoTotal = (radioEmpoderado + empoderadoNivel * nivel) / 100;
        float aceleradoTotal = (radioAcelerado + aceleradoNivel * nivel) / 100;
        float curaTotal = (radioCura + curaNivel * nivel) / 100;
        estados.AplicarEmpoderado(duracion, valor * empoderadoTotal);
        estados.AplicarAcelerado(aceleradoTotal, valor * aceleradoTotal);
        vida.RecibirCura(valor * curaTotal);
    }


    public override string GetDescripcion()
    {
        return "Recibir daño restaura una parte de la vida perdida, y además te empodera y acelera en proporción al daño recibido.";
    }
    public override string GetNombre() { return "Masoquismo"; }
}
