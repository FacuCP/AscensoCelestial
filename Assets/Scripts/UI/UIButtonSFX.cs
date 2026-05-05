using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    ISelectHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Esto cubre navegación con teclado/gamepad
        PlayHover();
    }

    private void PlayClick()
    {
        AudioManager.Instance?.PlaySFX(GameAssets.i.botonClick);
    }

    private void PlayHover()
    {
        AudioManager.Instance?.PlaySFX(GameAssets.i.botonHover);
    }
}