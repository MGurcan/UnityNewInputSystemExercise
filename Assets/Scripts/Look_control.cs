using UnityEngine;
using UnityEngine.InputSystem;
public class Look_control : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    Vector2 mouseMovement;
    float xrotation, yrotation;
    float camera_offset;
    float mouseSensivity = 20;
    void Start()
    {
        camera_offset = transform.position.y - player.transform.position.y - 10;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseMovement = Mouse.current.delta.ReadValue();
        xrotation += mouseMovement.y * Time.deltaTime * mouseSensivity;
        xrotation = Mathf.Clamp(xrotation, -30, 45);

        yrotation += mouseMovement.x * Time.deltaTime * mouseSensivity;
        transform.rotation = Quaternion.Euler(xrotation, yrotation, 0);

        player.transform.rotation = Quaternion.Euler(0, yrotation, 0);

    }
}
