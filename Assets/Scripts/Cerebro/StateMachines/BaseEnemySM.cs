using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class BaseEnemySM : StateMachine
{
    public Rigidbody Cuerpo { get; protected set; }
    public Estadisticas Stats { get; protected set; }
    public HandlerEstados Estados { get; protected set; }
    public HandlerAnimacion Animador { get; protected set; }
    public HandlerAtaque HandlerAtaque { get; protected set; }
    public HandlerPoder HandlerPoder { get; protected set; }
    public HandlerVelocidad HandlerVelocidad { get; protected set; }
    public HandlerVida HandlerVida { get; protected set; }

    [HideInInspector]public Vector3 direccion;
    public NavMeshAgent Agent { get; private set; }
    [SerializeField] private Transform jugador;
    public Transform Jugador => jugador;

    [SerializeField] protected int esencia = 30;
    [SerializeField] protected float rangoDetectarJugador = 15f; // empezar persecución
    [SerializeField] protected float rangoPerderJugador = 20f;   // volver a wander
    public float RangoDetectarJugador => rangoDetectarJugador;
    public float RangoPerderJugador => rangoPerderJugador;

    public virtual WanderEnemy Wander { get; protected set; }
    public virtual FollowEnemy Follow { get; protected set; }

    protected virtual void Awake()
    {
        Cuerpo = GetComponent<Rigidbody>();
        Stats = GetComponentInChildren<Estadisticas>();
        Estados = GetComponent<HandlerEstados>();
        Animador = GetComponentInChildren<HandlerAnimacion>();
        HandlerAtaque = GetComponentInChildren<HandlerAtaque>();
        HandlerPoder = GetComponentInChildren<HandlerPoder>();
        HandlerVelocidad = GetComponentInChildren<HandlerVelocidad>();
        HandlerVelocidad.CambioVelocidad += ActualizarVelocidad;
        HandlerVida = GetComponentInChildren<HandlerVida>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.acceleration = 1000f;   // o mayor
        Agent.autoBraking = false;    // opcional pero recomendado
        Wander = new WanderEnemy(this);
        Follow = new FollowEnemy(this);
        HandlerVida.Murio += Morir;
        ActualizarVelocidad();
    }

    private void ActualizarVelocidad()
    {
        Agent.speed = HandlerVelocidad.Velocidad;
    }

    public void OnEnable()
    {
        jugador = JugadorSM.Instancia.GetComponent<Transform>();
    }

    /// Movimiento Random limpio
    protected override BaseState GetInitialState(){ return Wander; }

    public void SetJugador(Transform player) { 
        if (Jugador == null) jugador = player;
    }
    public void MoverRandom(float radio)
    {
        if (Estados.Paralizado)
            return;

        Vector3 random = transform.position + UnityEngine.Random.insideUnitSphere * radio;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(random, out hit, radio, NavMesh.AllAreas))
        {
            // Direccion limpia hacia el nuevo punto
            direccion = (hit.position - transform.position).normalized;

            // Enviar al NavMeshAgent
            Agent.SetDestination(hit.position);
        }
    }

    public Action MuerteEnemigo;
    protected virtual void Morir() {
        JugadorSM.Instancia.AgregarEsencia(esencia);
        HandlerVida.Reiniciar();
        MuerteEnemigo?.Invoke();
        Destroy(gameObject);
    }
}
