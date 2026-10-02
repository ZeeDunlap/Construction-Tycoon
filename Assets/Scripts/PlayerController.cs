using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public int speed = 10;
    public double mouseSensitivity = 0.55;
    private float _horizontalInput, _verticalInput;
    
    
    private PlayerInput _input;

    // private Vector2 rotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = gameObject.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_horizontalInput * Time.deltaTime * speed * Vector3.right);
        transform.Translate(_verticalInput * Time.deltaTime * speed * Vector3.forward);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var movement = ctx.ReadValue<Vector2>();
        _horizontalInput = movement.x;
        _verticalInput = movement.y;
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        Ray ray = _input.camera.ScreenPointToRay(ctx.ReadValue<Vector2>());     
        // this.rotation = rotation;
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
        }
    }

    public void OnControlChanged(PlayerInput input)
    {
        Debug.Log("Player Controls Changed: " + input.currentActionMap.name);
        this._input = input;
    }
}
