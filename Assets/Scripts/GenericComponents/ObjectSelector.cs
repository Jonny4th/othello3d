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
        Debug.Log($"click pos {m_MousePosition}");
        Ray ray = Camera.main.ScreenPointToRay(m_MousePosition);

        // Perform the raycast
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            // The 'hit' variable contains information about what was hit
            GameObject clickedObject = hit.collider.gameObject;
            Debug.Log("Clicked on: " + clickedObject.name);

            // You can now perform actions on the clicked object
            // For example, calling a method on a script attached to it:
            clickedObject.GetComponentInParent<ISelectable>()?.Select();
        }
    }

    //void Update()
    //{
    //    // Check if the left mouse button was clicked down
    //    if(Input.GetMouseButtonDown(0))
    //    {
    //        // Create a ray from the camera through the mouse position
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        // Perform the raycast
    //        if(Physics.Raycast(ray, out hit))
    //        {
    //            // The 'hit' variable contains information about what was hit
    //            GameObject clickedObject = hit.collider.gameObject;
    //            Debug.Log("Clicked on: " + clickedObject.name);

                
    //            // You can now perform actions on the clicked object
    //            // For example, calling a method on a script attached to it:
    //            clickedObject.GetComponent<ISelectable>().Select();
    //        }
    //    }
    //}
}

public interface ISelectable
{
    void Select();
}
