using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public float sensitivity = 10f;
	public float speed = 12f;

	public Camera cam;

	float xRotation = 0f;

	InputAction lookAction;
	InputAction moveAction;

	bool isUsingGamepad => lookAction.activeControl?.device is Gamepad;

	CharacterController controller;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		controller = GetComponent<CharacterController>();
		InputInit();
    }

    void Update()
    {
		CameraLook();
		MovePlayer();
    }

	void InputInit()
	{
		lookAction = InputSystem.actions.FindAction("Look");
		moveAction = InputSystem.actions.FindAction("Move");
	}

	void CameraLook()
	{
		Vector2 input = lookAction.ReadValue<Vector2>() * sensitivity * Time.deltaTime * (isUsingGamepad ? 20 : 1);

		xRotation -= input.y;
		xRotation = Mathf.Clamp(xRotation, -75f, 75f);

		cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		transform.Rotate(Vector3.up * input.x);
	}

	void MovePlayer()
	{
		Vector2 input = moveAction.ReadValue<Vector2>();

		Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

		controller.Move(moveDirection * speed * Time.deltaTime);
	}
}
