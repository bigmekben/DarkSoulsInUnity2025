using UnityEngine;

namespace SG
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool circleInput;
        public bool rbInput;
        public bool rtInput;


        public bool rollFlag;
        public bool sprintFlag;
        public float rollInputTimer;

        InputSystem_Actions inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
        }

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new InputSystem_Actions();
                inputActions.Player.Move.performed += inputAction => movementInput = inputAction.ReadValue<Vector2>();
                inputActions.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleCircleInput(delta);
            HandleAttackInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleCircleInput(float delta)
        {
            circleInput = inputActions.Player.CircleButton.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if(circleInput)
            {
                rollInputTimer += delta;
                if (moveAmount > 0.5f)
                {
                    sprintFlag = true;
                }
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleAttackInput(float delta)
        {
            inputActions.Player.RB.performed += i => rbInput = true;
            inputActions.Player.RT.performed += i => rtInput = true;

            if(rbInput)
            {
                if(!playerAttacker.LightAttackBusy())
                {
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }
            }   
            
            if (rtInput)
            {
                if(!playerAttacker.HeavyAttackBusy())
                {
                    playerAttacker.HandleHeavyAttack(playerInventory.leftWeapon);
                }
            }
        }
    }
}
