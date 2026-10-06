using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public int speed = 130;
    public float mouseSensitivity = 0.55f;
    private float _horizontalInput, _verticalInput;

    private Vector3 _rotation;
    
    private PlayerInput _input;
    private CharacterController _controller;
    private Animator _animator;

    // private Vector2 rotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = gameObject.GetComponent<CharacterController>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        _input = gameObject.GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        var turning = _horizontalInput * speed * gameObject.transform.right;
        var forward = _verticalInput * speed * gameObject.transform.forward;
        _controller.Move(new Vector3(turning.x + forward.x , _controller.isGrounded ? 0.0f : -32.0f, 
                         forward.z + turning.z) * Time.deltaTime);
        // transform.rotation = _input.camera.transform.rotation;
        _animator.SetBool("IsGrounded", _controller.isGrounded);
        var lookRotation = _rotation + _input.camera.transform.rotation.eulerAngles;
        // lookRotation.x = Mathf.Clamp(lookRotation.x, -90f, 90f);
        _input.camera.transform.Rotate(new Vector3(_rotation.x, 0f, 0f));
        gameObject.transform.Rotate(new Vector3(0, _rotation.y, 0));
        // _input.camera.transform.Rotate(_rotation);
        Ray ray = _input.camera.ScreenPointToRay(_input.camera.rect.center);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 50.0f) && hit.collider.gameObject == gameObject)
        {
            Debug.Log("Looking at Pipe!!!");
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var movement = ctx.ReadValue<Vector2>();
        if (movement.x > 0)
        {
            _animator.SetTrigger("ForwardPressed");
            _animator.ResetTrigger("BackwardPressed");
        } else if (movement.x < 0)
        {
            _animator.SetTrigger("BackwardPressed");
            _animator.ResetTrigger("ForwardPressed");
        }

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
        // _controller.transform.Rotate(new Vector3(0f, direction.x,0f));
        // Only apply y value to camera since the player will otherwise fall over
        
        //Player should not be allowed to rotate their camera 360 degrees up and down
        // Debug.Log("Rotation is: " + _input.camera.transform.rotation.eulerAngles);
        var upDownRotation = -direction.y;
        var leftRightRotation = direction.x;
        _rotation = new Vector3(upDownRotation, leftRightRotation, 0f);
    }
    
}
