using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance { get; private set; }

    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        Instance = this;   
    }
    private void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 999f, layerMask))
            {
                transform.position = hit.point;
            }
        }
    }

    public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();
    private Vector3 GetMouseWorldPosition_Instance() {
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 999f, layerMask))
            {
                return hit.point;
            }
            else { return Vector3.zero; }
        }
        else { return new Vector3(); }  
    }
}
