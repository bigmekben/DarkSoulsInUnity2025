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

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryFlag;
        public float rollInputTimer;

        InputSystem_Actions inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindFirstObjectByType<UIManager>();
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
            HandleQuickSlotsInput();
            HandleInteractingButtonInput();
            HandleYTriangleInput();
            HandleInventoryInput();
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
    }
}
