using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    public Transform popUpVida;

    [Header("Musica")]
    public AudioClip musicaMenu;
    public AudioClip musicaNiveles;
    public AudioClip musicaJefe;

    [Header("SFX Generales")]
    public AudioClip dmgTomado;
    public AudioClip dmgCurado;
    public AudioClip epifaniasFinNivel;
    public AudioClip castAtaque;
    public AudioClip castPoder;
    public AudioClip golpePoder;
    public AudioClip botonClick;
    public AudioClip botonHover;
    public AudioClip teleport;
    public AudioClip morir;

    [Header("Prefabs Habilidades")]
    public List<PoderBase> poderes;
    public List<ForjaBase> forjas;
    public List<FavorBase> favores;
    public List<BlasfemiaBase> blasfemias;

    [Header("Habilidades Para Epifanias")]
    public List<Habilidad> poderesEpifanias;
    public List<Habilidad> forjasEpifanias;
    public List<Habilidad> favoresEpifanias;
    public List<Habilidad> blasfemiasEpifanias;
}
