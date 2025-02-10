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

        public bool bInput;
        public bool aInput;
        public bool yInput;
        public bool xInput;
        public bool rbInput;
        public bool rtInput;
        public bool lbInput;
        public bool ltInput;
        public bool dPadUp;
        public bool dPadDown;
        public bool dPadLeft;
        public bool dPadRight;
        public bool inventoryInput;
        public bool l3Input;
        public bool r3Input;
        public bool rightStickRightInput;
        public bool rightStickLeftInput;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool inventoryFlag;
        public float rollInputTimer;

        InputSystem_Actions inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindFirstObjectByType<UIManager>();
            cameraHandler = FindFirstObjectByType<CameraHandler>();
        }

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new InputSystem_Actions();
                inputActions.Player.Move.performed += inputAction => movementInput = inputAction.ReadValue<Vector2>();
                inputActions.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.Player.RB.performed += i => rbInput = true;
                inputActions.Player.RT.performed += i => rtInput = true;
                inputActions.Player.Previous.performed += i => dPadLeft = true;
                inputActions.Player.Next.performed += i => dPadRight = true;
                inputActions.Player.XButton.performed += i => aInput = true;
                inputActions.Player.TriangleButton.performed += i => yInput = true;
                inputActions.Player.OptionsButton.performed += i => inventoryInput = true;
                inputActions.Player.L3.performed += i => l3Input = true;
                inputActions.Player.R3.performed += i => r3Input = true;
                inputActions.Player.LockOnTransformRight.performed += i => rightStickRightInput = true;
                inputActions.Player.LockOnTransformLeft.performed += i => rightStickLeftInput = true;
            }
            inputActions.Enable();
        }

        private void R3_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
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
            HandleQuickSlotsInput();
            HandleInteractingButtonInput();
            HandleYTriangleInput();
            HandleInventoryInput();
            HandleLockOnInput();
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
            bInput = inputActions.Player.CircleButton.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            sprintFlag = bInput;
            if (bInput)
            {
                rollInputTimer += delta;
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

            if(rbInput)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    if (playerManager.isInteracting)
                    {
                        return;
                    }
                    if (playerManager.canDoCombo)
                    {
                        return;
                    }
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }
            }

            if (rtInput)
            {
                if (playerManager.isInteracting)
                {
                    return;
                }
                if (playerManager.canDoCombo)
                {
                    return;
                }
                if (playerManager.canDoCombo)
                {

                }

                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }

        private void HandleQuickSlotsInput()
        {
            if (dPadRight)
            {
                playerAttacker.EquipRightWaist();
                // to do: attach a "holster" object to the player's waist.  EquipRightWaist should tell it 
                // which model to display depending on the recently-unequipped weaponItem.  So should pass some params to EquipRightWaist after all.
                // should be fine for now though.
                playerInventory.ChangeRightWeapon();
            }
            if (dPadLeft)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInteractingButtonInput()
        {

        }

        private void HandleYTriangleInput()
        {
        }

        private void HandleInventoryInput()
        {
            if(inventoryInput)
            {
                inventoryFlag = !inventoryFlag;

                if(inventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.hudWindow.SetActive(true);
                }
            }
        }

        private void HandleLockOnInput()
        {
            if(r3Input && !lockOnFlag)
            {
                r3Input = false;
                cameraHandler.HandleLockOn();
                if(cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if (r3Input && lockOnFlag)
            {
                r3Input = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
            }
            if(lockOnFlag && rightStickLeftInput)
            {
                rightStickLeftInput = false;
                cameraHandler.HandleLockOn();
                if(cameraHandler.leftLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (lockOnFlag && rightStickRightInput)
            {
                rightStickRightInput = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }
        }
    }
}
