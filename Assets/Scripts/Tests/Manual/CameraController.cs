using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellwright.Tests.Manual
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private const float MIN_VERTICAL_ANGLE = -89f;
        private const float MAX_VERTICAL_ANGLE = 89f;

        [Header("Movement")]
        [SerializeField]
        private float _moveSpeed = 10f;

        [SerializeField]
        private float _sprintMultiplier = 2f;

        [Header("Movement Keys")]
        [SerializeField]
        private Key _forwardKey = Key.W;

        [SerializeField]
        private Key _backwardKey = Key.S;

        [SerializeField]
        private Key _leftKey = Key.A;

        [SerializeField]
        private Key _rightKey = Key.D;

        [SerializeField]
        private Key _upKey = Key.E;

        [SerializeField]
        private Key _downKey = Key.Q;

        [SerializeField]
        private Key _sprintKey = Key.LeftShift;

        [Header("Rotation")]
        [SerializeField]
        private bool _lockCursorOnStart = true;

        [SerializeField]
        private Key _unlockCursorKey = Key.Escape;

        [SerializeField]
        private float _mouseSensitivity = 2f;

        private float _rotationX;
        private float _rotationY;
        private bool _cursorLocked;

        [SuppressMessage("Style", "IDE0051:Remove unused private members")]
        private void Start()
        {
            Vector3 rotation = transform.eulerAngles;
            _rotationX = rotation.y;
            _rotationY = rotation.x;

            if (_lockCursorOnStart)
            {
                LockCursor();
            }
        }

        [SuppressMessage("Style", "IDE0051:Remove unused private members")]
        private void Update()
        {
            HandleCursorToggle();
            HandleMovement();
            HandleRotation();
        }

        private void HandleCursorToggle()
        {
            if (Keyboard.current == null || !Keyboard.current[_unlockCursorKey].wasPressedThisFrame)
            {
                return;
            }

            if (_cursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }

        private void LockCursor()
        {
            _cursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockCursor()
        {
            _cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void HandleMovement()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            Vector3 input = CalculateMovementInput();

            if (input.sqrMagnitude > 0.01f)
            {
                ApplyMovement(input);
            }
        }

        private Vector3 CalculateMovementInput()
        {
            Vector3 input = Vector3.zero;

            if (Keyboard.current[_forwardKey].isPressed)
            {
                input += Vector3.forward;
            }

            if (Keyboard.current[_backwardKey].isPressed)
            {
                input += Vector3.back;
            }

            if (Keyboard.current[_leftKey].isPressed)
            {
                input += Vector3.left;
            }

            if (Keyboard.current[_rightKey].isPressed)
            {
                input += Vector3.right;
            }

            if (Keyboard.current[_upKey].isPressed)
            {
                input += Vector3.up;
            }

            if (Keyboard.current[_downKey].isPressed)
            {
                input += Vector3.down;
            }

            return input;
        }

        private void ApplyMovement(Vector3 input)
        {
            input.Normalize();

            float speed = Keyboard.current[_sprintKey].isPressed ? _moveSpeed * _sprintMultiplier : _moveSpeed;

            Vector3 targetVelocity = transform.TransformDirection(input) * speed;
            transform.position += targetVelocity * Time.deltaTime;
        }

        private void HandleRotation()
        {
            if (Mouse.current == null || !_cursorLocked)
            {
                return;
            }

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            _rotationX += mouseDelta.x * _mouseSensitivity;
            _rotationY -= mouseDelta.y * _mouseSensitivity;
            _rotationY = Mathf.Clamp(_rotationY, MIN_VERTICAL_ANGLE, MAX_VERTICAL_ANGLE);

            transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }
    }
}
