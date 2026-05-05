using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HandlerEstados : MonoBehaviour
{
    private bool invencible = false;
    private bool inmortal = false;
    private bool invisible = false; 
    private bool imparable = false;
    private bool ciego = false; 
    private bool paralizado = false;

    public event Action<float,float> OnMaldito;
    public event Action<float, float> OnBendito;
    public event Action<float, float> OnRalentizado;
    public event Action<float, float> OnAcelerado;
    public event Action<float, float> OnDebilitado;
    public event Action<float, float> OnEmpoderado;
    public bool Imparable => imparable;
    public bool Invencible => invencible;
    public bool Inmortal => inmortal; 
    public bool Invisible => invisible;
    public bool Ciego => ciego;
    public bool Paralizado => paralizado;

    public event Action<TipoEstado> OnFinEstado;

    public event Action<TipoEstado> OnInicioEstado;

    public void AplicarBendito(float tiempo, float valor) {
        OnBendito?.Invoke(tiempo,valor / 100);
        OnInicioEstado?.Invoke(TipoEstado.Bendito);
    }
    public void AplicarMaldito(float tiempo, float valor) { 
        if (imparable) return;
        OnMaldito?.Invoke(tiempo,valor / 100);

        OnInicioEstado?.Invoke(TipoEstado.Maldito);
    }
    public void AplicarRalentizado(float tiempo, float valor) { 
        if (imparable) return;
        OnRalentizado?.Invoke(tiempo,valor / 100);
        OnInicioEstado?.Invoke(TipoEstado.Ralentizado);
    }
    public void AplicarAcelerado(float tiempo, float valor) { 
        OnAcelerado?.Invoke(tiempo, valor / 100);

        OnInicioEstado?.Invoke(TipoEstado.Acelerado);
    }
    public void AplicarDebilitado(float tiempo, float valor)
    {
        if(imparable) return;
        OnDebilitado?.Invoke(tiempo, valor / 100);
        OnInicioEstado?.Invoke(TipoEstado.Debilitado);
    }
    public void AplicarEmpoderado(float tiempo, float valor)
    {
        OnEmpoderado?.Invoke(tiempo, valor / 100);
        OnInicioEstado?.Invoke(TipoEstado.Empoderado);
    }

    public void AplicarInvencible(float tiempo)
    {
        AplicarEstado(tiempo, TipoEstado.Invencible, valor => invencible = valor);
    }
    public void AplicarImparable(float tiempo)
    {
        AplicarEstado(tiempo, TipoEstado.Imparable, valor => imparable = valor);
    }
    public void AplicarInmortal(float tiempo)
    {
        AplicarEstado(tiempo, TipoEstado.Inmortal, valor => inmortal = valor);
    }
    public void AplicarInvisible(float tiempo) { 
        AplicarEstado(tiempo, TipoEstado.Invisible,valor => invisible = valor); 
    }

    public void AplicarCiego(float tiempo)
    {
        if (imparable || ciego) return;
        AplicarEstado(tiempo, TipoEstado.Ciego,valor => ciego = valor);
    }

    public void AplicarParalizado(float tiempo)
    {
        if (imparable || paralizado) return;
        AplicarEstado(tiempo, TipoEstado.Paralizado, valor => paralizado = valor);
    }

    private void AplicarEstado(float tiempo, TipoEstado tipo, Action<bool> setEstado)
    {
        setEstado(true);
        OnInicioEstado?.Invoke(tipo);
        StartCoroutine(Esperar(tiempo, () =>
        {
            setEstado(false);
            OnFinEstado?.Invoke(tipo);
        }));
    }

    public void FinEstado(TipoEstado tipo)
    {
        OnFinEstado?.Invoke(tipo);
    }

    private IEnumerator Esperar(float tiempo, System.Action alFinalizar)
    {
        yield return new WaitForSeconds(tiempo);
        alFinalizar?.Invoke();
    }

    public void Reiniciar()
    {
        StopAllCoroutines();
        invencible = false;
        inmortal = false;
        invisible = false;
        imparable = false;
        ciego = false;
        paralizado = false;
    }
}
public enum TipoEstado
{
    Invencible,
    Inmortal,
    Imparable,
    Invisible,
    Ciego,
    Paralizado,
    Maldito,
    Bendito,
    Ralentizado,
    Acelerado,
    Debilitado,
    Empoderado
}
