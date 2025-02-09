using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{
    public class PlayerManager : MonoBehaviour
    {

        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemInteractableGameObject;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;

        private void Awake()
        {
            cameraHandler = FindFirstObjectByType<CameraHandler>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            interactableUI = FindFirstObjectByType<InteractableUI>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);

            inputHandler.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection); // couldn't HandleFalling just access moveDirection directly?
            playerLocomotion.HandleJumping();
            CheckForInteractableObject();
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            inputHandler.rbInput = false;
            inputHandler.rtInput = false;
            inputHandler.lbInput = false;
            inputHandler.ltInput = false;
            inputHandler.dPadLeft = false;
            inputHandler.dPadRight = false;
            inputHandler.dPadUp = false;
            inputHandler.dPadDown = false;
            inputHandler.aInput = false;
            inputHandler.bInput = false;
            inputHandler.yInput = false;
            inputHandler.xInput = false;

            if (isInAir)
            {
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }

        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);
                        if (inputHandler.aInput)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }
            else
            {
                if(interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }

                if(itemInteractableGameObject != null && inputHandler.aInput)
                {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }
    }
}
