using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public bool disableHeadbob = true;

	public float sensitivity = 10f;
	public float walkSpeed = 4f;
	public float sprintSpeed = 10f;
	public float jumpHeight = 18f;
	public float groundDistance = 0.1f;

	public bool canMove = false;

	public float health { get; private set; }

	public Camera cam;
	public Animator anim;
	public Transform groundCheck;
	public LayerMask groundMask;

	const int STATE_IDLE = 0;
	const int STATE_MOVING = 1;
	const int STATE_SPRINTING = 2;
	const int STATE_DEFAULT = 3;

	const float JOYSTICK_DEADZONE = 0.125f;

	float xRotation = 0f;
	float yRotation = 0f;

	float speed;

	InputAction lookAction;
	InputAction moveAction;
	InputAction jumpAction;
	InputAction sprintAction;

	bool isUsingGamepad => lookAction.activeControl?.device is Gamepad || moveAction.activeControl?.device is Gamepad;

	bool isSprinting;

	Vector3 velocity;

	CharacterController controller;

	const float GRAVITY = -9.81f * 0.005f;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		controller = GetComponent<CharacterController>();
		InputInit();
    }

    void Update()
    {
		if (!canMove) return;
		Headbob();
		CameraLook();
		ApplyGravity();
		HandleSprint();
		MovePlayer();
    }

	void InputInit()
	{
		lookAction = InputSystem.actions.FindAction("Look");
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");
		sprintAction = InputSystem.actions.FindAction("Sprint");
	}

	void Headbob()
	{
		if (disableHeadbob == true)
		{
			anim.SetInteger("state", STATE_DEFAULT);
			return;
		}

		Vector2 input = moveAction.ReadValue<Vector2>();

		int state = input.magnitude >= 0.125f ?
			(isSprinting ? STATE_SPRINTING : STATE_MOVING) 
			: STATE_IDLE;
		anim.SetInteger("state", state);
	}

	void CameraLook()
	{
		Vector2 input = lookAction.ReadValue<Vector2>() * sensitivity * (isUsingGamepad ? 25f * Time.deltaTime : 0.01f);

		xRotation -= input.y;
		yRotation += input.x;

		xRotation = Mathf.Clamp(xRotation, -75f, 75f);

		cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
	}

	void MovePlayer()
	{
		Vector2 input = moveAction.ReadValue<Vector2>();

		Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

		speed = isSprinting ? sprintSpeed : walkSpeed;
		controller.Move(moveDirection * speed * Time.deltaTime * 0.15f);

		if (isGrounded() && velocity.y < 0)
		{
			velocity.y = 0f;
		}

		if (jumpAction.WasPressedThisFrame() && isGrounded())
		{
			Jump();
		}

		controller.Move(velocity * Time.deltaTime);
	}

	void HandleSprint()
	{
		Vector2 input = moveAction.ReadValue<Vector2>();

		if (isUsingGamepad)
		{
			// Toggle Sprint for controller
			if (sprintAction.WasPerformedThisFrame() || sprintAction.IsPressed()) isSprinting = true;
			else if (input.magnitude < JOYSTICK_DEADZONE) isSprinting = false;
		}
		else
		{
			// Hold Sprint for keyboard
			if (sprintAction.IsPressed()) isSprinting = true;
			else isSprinting = false;
		}
	}

	void Jump() { velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY); }

	bool isGrounded() { return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); }

	void ApplyGravity() { velocity.y += GRAVITY*(isGrounded() ? 0 : 1); }

	public void AddHealth(int value) { health += value; }

	public void TakeDamage(int value) { health -= value; }
}
