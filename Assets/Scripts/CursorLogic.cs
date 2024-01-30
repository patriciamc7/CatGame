using UnityEngine;

public class CursorLogic : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window
    }
    void FixedUpdate()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(worldPosition.x, 0, 0);
    }
}