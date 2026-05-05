using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    private Controles controles;
    private JugadorSM controller;

    private void Awake()
    {
        controles = new();
        controller = GetComponent<JugadorSM>();
    }

    private void OnEnable()
    {
        controles.Enable();
        controles.Base.Ataque.started += ctx =>
        {
            controller.OnInputAtaque();
        };
        controles.Base.Poder.started += ctx => controller.OnInputPoder();
        controles.Base.Bendicion.started += ctx =>
        {
            if (DialogoController.Instancia != null && DialogoController.Instancia.DialogoActivo) {
                DialogoController.Instancia.Siguiente();
            }
            else if(LevelManager.Nivel == 0 && AreaDecreto.PlayerInArea)
            {
                PanelDecretos.Instancia.Activar();
            }else if(!DialogoController.Instancia.DialogoActivo && AreaLucifer.PlayerInArea)
            {
                AreaLucifer.Instancia.IniciarDialogo();
            }
            else
            {
                controller.OnInputFavor();
            }
     
        };
        controles.Base.Cambio.started += ctx => controller.OnInputCambio(controles.Base.Cambio.ReadValue<Vector2>().y);
        controles.Base.Escape.started += ctx => MenuPausa.Instancia.ToggleMenu();
        controles.Base.Epifanias.started += ctx => ControlPantallaEpifanias.Instancia.Inicializar();

        controles.Base.CambiarPoderes.performed += ctx => controller.OnInputNumero(GetNumeroDesdeInput(ctx));
    }

    private void Update()
    {
        controller.OnInputMovimiento(controles.Base.Mover.ReadValue<Vector3>());
    }

    private void OnDisable()
    {
        controles.Disable();
    }

    private int GetNumeroDesdeInput(InputAction.CallbackContext ctx)
    {
        var keyControl = ctx.control as KeyControl;
        if (keyControl == null) return -1;

        return keyControl.keyCode switch
        {
            Key.Digit0 or Key.Numpad0 => 10,
            Key.Digit1 or Key.Numpad1 => 1,
            Key.Digit2 or Key.Numpad2 => 2,
            Key.Digit3 or Key.Numpad3 => 3,
            Key.Digit4 or Key.Numpad4 => 4,
            Key.Digit5 or Key.Numpad5 => 5,
            Key.Digit6 or Key.Numpad6 => 6,
            Key.Digit7 or Key.Numpad7 => 7,
            Key.Digit8 or Key.Numpad8 => 8,
            Key.Digit9 or Key.Numpad9 => 9,
            _ => -1
        };
    }

}
