using UnityEditor.ShaderApiReflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public int speed = 130;
    public float mouseSensitivity = 0.55f;
    private float _horizontalInput, _verticalInput;

    private Vector3 _rotation;
    
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
        _input.camera.transform.Rotate(_rotation);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var movement = ctx.ReadValue<Vector2>();
        _horizontalInput = movement.x;
        _verticalInput = movement.y;
    }

    public void OnControlChanged(PlayerInput input)
    {
        Debug.Log("Player Controls Changed: " + input.currentActionMap.name);
        this._input = input;
    }
    
    public void ReadMouseInput(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 objectPosition = (Vector2) _input.camera.WorldToScreenPoint(transform.position);
        Vector2 direction = (mousePosition - objectPosition).normalized;

        RotateAim(direction);
    }

    public void ReadStick(InputAction.CallbackContext context)
    {
        Vector2 stickDirection = context.ReadValue<Vector2>().normalized;

        RotateAim(stickDirection);
    }

    public void RotateAim(Vector2 direction)
    {
        // Debug.Log("Direction is: " + direction);
        // X is the value that determines Left and right (+1 = Left, -1 = Right)
        _rotation = new Vector3();
        transform.Rotate(new Vector3(0f, direction.x,0f));
        // Only apply y value to camera since the player will otherwise fall over
        
        //Player should not be allowed to rotate their camera 360 degrees up and down
        Debug.Log("Rotation is: " + _input.camera.transform.rotation.eulerAngles);
        var newRotation = _input.camera.transform.rotation.x + (-direction.y);
        if (newRotation > -1.5 && newRotation < 1.5)
        {
            Debug.Log("New Rotation is: " + newRotation);
            _rotation = new Vector3(-direction.y, 0f, 0f);
        }
    }
    
}
