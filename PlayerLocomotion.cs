using UnityEngine;

// As of episode 7, this has a few tweaks that Ben made over what was shown in the tutorial video.
// As of this check-in, the behavior isn't perfect; the player gets stuck on edge, doesn't walk up stairs properly, etc.
// Will try to tweak it later.  Not sure if there's something wrong with the animation, size of capsule collider, parameters to this script, etc.

namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;


        public new Rigidbody rigidbody; // tbd: is "new" needed?
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        [Tooltip("body's distance from center to toe")]
        float toeDepth = 0.4f; // tutorial uses 0.4.
        [SerializeField]
        [Tooltip("minimum height of obstacle you can't simply walk up - typically just over the body's ankle bone")]
        float tooHighStepHeight = 0.4f; // tutorial calls this "groundDetectionRayStartPoint"
        // The stairs in my house are 7 5/8", or about an inch above my ankle bone;
        // so I got 0.4 on my model by positioning the bottom of the capsule collider .
        [SerializeField]
        [Tooltip("if body was falling for at least this long, it will switch to a landing anim when touching ground")]
        float minimumFallTime = 0.5f;
        [SerializeField]
        [Tooltip("while falling, movement speed will be divided by this number")]
        float fallDampening = 2f;

        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f; 
        // player is 1.79 units tall; so this is falling from about hip height.
        // knee height would be 0.54.  Would be nice to add a secondary land animation for falling from knee-to-hip height.

        [SerializeField]
        float groundDirectionRayDistance = 0.2f;

        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 4f;
        [SerializeField]
        float sprintSpeed = 7f;
        [SerializeField]
        float rotationSpeed = 10f;
        [SerializeField]
        float fallingSpeed = 80f;

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }


        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if(inputHandler.rollFlag)
            {
                return;
            }

            if(playerManager.isInteracting)
            {
                return;
            }

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed * inputHandler.moveAmount;

            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
            }
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.linearVelocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
            {
                return;
            }

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += tooHighStepHeight;

            Debug.DrawRay(origin, myTransform.forward * toeDepth, Color.red);
            // Prevent lateral movement if the step height or slope is too high or would look awkward
            // (or if you are falling while pressing up to a wall or cliff):
            if(Physics.Raycast(origin, myTransform.forward, out hit, toeDepth))
            {
                moveDirection = Vector3.zero;
            }

            // Simulate gravity's work, with a 20% forward momentum.
            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 5f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin += dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            // stop vertical movement when feet touch ground/platform:
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                // if body was falling for long enough, show landing animation;
                // otherwise let the animation stay the same.
                if(playerManager.isInAir)
                {
                    if(inAirTimer > minimumFallTime)
                    {
                        Debug.Log($"You were in the air for {inAirTimer}");
                        animatorHandler.PlayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Locomotion", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                // Nothing under the body's feet in lateral movement direction:
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                // Only start the anim and set the new velocity once, when fall begins:
                if(playerManager.isInAir == false)
                {
                    if(playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 vel = rigidbody.linearVelocity;
                    vel.Normalize();
                    rigidbody.linearVelocity = vel * (movementSpeed / fallDampening);
                    playerManager.isInAir = true; // serves as a debounce.
                }
            }

            // body may have just "landed" (touched ground) during this frame, or where already on the ground from a
            // previous frame:
            if (playerManager.isGrounded)
            {
                if(playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
                    // Not sure this block does much; the only difference would be the y of targetPosition.
                    // I suppose the point is to ease into the collision point (only the y coordinate would be impacted).
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }
        }

        #endregion
    }
}
