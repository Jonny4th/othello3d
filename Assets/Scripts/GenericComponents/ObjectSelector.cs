using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSelector : MonoBehaviour
{
    Vector2 m_MousePosition;

    public void OnMouseMove(InputAction.CallbackContext callback)
    {
        m_MousePosition = callback.ReadValue<Vector2>();
    }

    public void OnMouseClick(InputAction.CallbackContext callback)
    {
        if(callback.phase != InputActionPhase.Performed) return;
        Ray ray = Camera.main.ScreenPointToRay(m_MousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            clickedObject.GetComponentInParent<ISelectable>()?.Select();
        }
    }
}

public interface ISelectable
{
    void Select();
}
