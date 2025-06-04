using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player
{
	public class SimpleRtsCamera : MonoBehaviour
	{
		[Header("RTS Camera Settings")]
		[Header("Move")]
		[SerializeField] private float moveSpeed = 200;
		[Tooltip("0 = edgescroll disabled")]
		[SerializeField] private float edgeThreshold = 5;
		[SerializeField] private float rightMouseSpeedMultiplier = 10;
		
		[Header("Zoom")]
		[SerializeField] private float zoomSpeed = 200;
		
		[Header("Rotate")]
		[SerializeField] private float rotateSpeed = 0.5f;
		[SerializeField] private float rotateFallback = 1000f;

		private PlayerInput _playerInput;
		private Vector2 _moveInput;
		private Vector2 _mousePositionInput;
		private float _rightMouseInput;
		private Vector2 _initialMousePosition;
		private float _scrollMouseInput;
		private float _middleMouseInput;

		private Vector3 _cameraForward;
		private Vector3 _cameraRight;
		private bool _isDragging;
		private float heightMultiplyer;
		
		private void Awake()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
		}

		private void OnEnable()
		{
			_playerInput.actions["CameraMove"].performed += MoveHandler;
			_playerInput.actions["CameraMove"].canceled += MoveHandler;

			_playerInput.actions["MousePosition"].performed += MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled += MousePositionHandler;

			_playerInput.actions["RightMouse"].started += InitialMousePositionHandler;
			_playerInput.actions["RightMouse"].performed += RightMouseHandler;
			_playerInput.actions["RightMouse"].canceled += RightMouseHandler;

			_playerInput.actions["ScrollMouse"].performed += ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled += ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started += InitialMousePositionHandler;
			_playerInput.actions["MiddleMouse"].performed += MiddleMouseHandler;
			_playerInput.actions["MiddleMouse"].canceled += MiddleMouseHandler;
		}

		private void LateUpdate()
		{
			UpdateRelativeCameraVectors();
			heightMultiplyer = transform.position.y;
			MoveCamera();
			MoveCameraWithCursor();
			MoveCameraWithRightMouse();
			ZoomCamera();
			RotateCamera();
		}

		private void OnDisable()
		{
			if (!_playerInput) return;

			_playerInput.actions["CameraMove"].performed -= MoveHandler;
			_playerInput.actions["CameraMove"].canceled -= MoveHandler;

			_playerInput.actions["MousePosition"].performed -= MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled -= MousePositionHandler;

			_playerInput.actions["RightMouse"].started -= InitialMousePositionHandler;
			_playerInput.actions["RightMouse"].performed -= RightMouseHandler;
			_playerInput.actions["RightMouse"].canceled -= RightMouseHandler;

			_playerInput.actions["ScrollMouse"].performed -= ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled -= ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started -= InitialMousePositionHandler;
			_playerInput.actions["MiddleMouse"].performed -= MiddleMouseHandler;
			_playerInput.actions["MiddleMouse"].canceled -= MiddleMouseHandler;
		}

		private void MoveHandler(InputAction.CallbackContext callbackContext) =>
			_moveInput = callbackContext.ReadValue<Vector2>();

		private void MousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_mousePositionInput = callbackContext.ReadValue<Vector2>();

		private void InitialMousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_initialMousePosition = _mousePositionInput;

		private void RightMouseHandler(InputAction.CallbackContext callbackContext) {
			_rightMouseInput = callbackContext.ReadValue<float>();
			
			if (callbackContext.performed)
			{
				_isDragging = true;
			} else if (callbackContext.canceled)
			{
				_isDragging = false;
			}
		}
		
		private void ScrollMouseHandler(InputAction.CallbackContext callbackContext) =>
			_scrollMouseInput = callbackContext.ReadValue<float>();

		private void MiddleMouseHandler(InputAction.CallbackContext callbackContext)
		{
			_middleMouseInput = callbackContext.ReadValue<float>();
			
			if (callbackContext.performed)
			{
				_isDragging = true;
			} else if (callbackContext.canceled)
			{
				_isDragging = false;
			}
		}

		private void UpdateRelativeCameraVectors()
		{
			var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
			var right = new Vector3(transform.right.x, 0, transform.right.z);
			forward.Normalize();
			right.Normalize();
			_cameraForward = forward;
			_cameraRight = right;
		}
		
		private void MoveCamera()
		{
			Vector3 moveDirection = (_cameraRight * _moveInput.x + _cameraForward * _moveInput.y) * 
			                        (heightMultiplyer * moveSpeed * Time.deltaTime);
			transform.position += moveDirection;
		}

		private void MoveCameraWithCursor()
		{
			
			if (_isDragging || edgeThreshold == 0)
				return; 
			
			if (_mousePositionInput.x < edgeThreshold)
			{
				transform.position += _cameraRight * (Vector3.left.x * (heightMultiplyer * moveSpeed * Time.deltaTime));
			}
			else if (_mousePositionInput.x > Screen.width - edgeThreshold)
			{
				transform.position += _cameraRight * (Vector3.right.x * (heightMultiplyer * moveSpeed * Time.deltaTime));
			}

			if (_mousePositionInput.y < edgeThreshold)
			{
				transform.position += _cameraForward * (Vector3.back.z * (heightMultiplyer * moveSpeed * Time.deltaTime));
			}
			else if (_mousePositionInput.y > Screen.height - edgeThreshold)
			{
				transform.position += _cameraForward * (Vector3.forward.z * (heightMultiplyer * moveSpeed * Time.deltaTime));
			}
		}

		private void MoveCameraWithRightMouse()
		{
			if (Mathf.Approximately(_rightMouseInput, 0) || _initialMousePosition == Vector2.zero) return;

			var mouseDelta = (_mousePositionInput - _initialMousePosition) * -1;

			Vector3 moveDirection = (_cameraRight * mouseDelta.x + _cameraForward * mouseDelta.y) * 
			                        ((heightMultiplyer * moveSpeed * rightMouseSpeedMultiplier / Screen.width) * Time.deltaTime);
			transform.position += moveDirection;

			_initialMousePosition = _mousePositionInput;
		}

		private void ZoomCamera()
		{
			if (Mathf.Approximately(_scrollMouseInput, 0)) return;

			var zoomDirection = transform.forward;
			transform.position += zoomDirection * (_scrollMouseInput * heightMultiplyer * zoomSpeed * Time.deltaTime);
		}

		private void RotateCamera()
		{
			if (Mathf.Approximately(_middleMouseInput, 0) || _initialMousePosition == Vector2.zero) return;

			var lookAtPoint = GetCameraLookAtPoint();
			var mouseDelta = _mousePositionInput - _initialMousePosition;
			transform.RotateAround(lookAtPoint, Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime);
			
			_initialMousePosition = _mousePositionInput;
		}

		private Vector3 GetCameraLookAtPoint()
		{
			var ray = new Ray(transform.position, transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
			{
				return hit.point;
			}

			var groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (groundPlane.Raycast(ray, out float enter))
			{
				return ray.GetPoint(enter);
			}

			return transform.position + transform.forward * rotateFallback;
		}
	}
}