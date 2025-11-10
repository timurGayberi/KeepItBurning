using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.8f;
        
        private CharacterController controller;
        private Vector3 moveInput;
        private Vector3 moveVelocity;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            controller.Move(move * Time.deltaTime * moveSpeed);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            Debug.Log(moveInput);
        }
    }
}
