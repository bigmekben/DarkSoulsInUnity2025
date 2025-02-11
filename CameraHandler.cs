using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;


        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform currentLockOnTarget;
        public float maximumLockOnDistance = 30f;
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindAnyObjectByType<PlayerManager>().transform;
            inputHandler = FindFirstObjectByType<InputHandler>();
            playerManager = FindFirstObjectByType<PlayerManager>();
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(
                myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollisions(delta);
            MaintainLockOnTargets();
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (!inputHandler.lockOnFlag && currentLockOnTarget == null)
            {
                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                float velocity = 0;
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;
                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();
                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        public void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        // BT Feb 11, 2025: prevent swapping to candidate targets behind walls, etc.
        public void MaintainLockOnTargets()
        {
            RaycastHit hit;
            if(leftLockTarget != null)
            {
                if (Physics.Linecast(playerManager.lockOnTransform.position, leftLockTarget.position, out hit))
                {
                    if (hit.transform.gameObject.layer == environmentLayer)
                    {
                        leftLockTarget = null;
                    }
                }
            }
            if (rightLockTarget != null)
            {
                if (Physics.Linecast(playerManager.lockOnTransform.position, rightLockTarget.position, out hit))
                {
                    if (hit.transform.gameObject.layer == environmentLayer)
                    {
                        rightLockTarget = null;
                    }
                }
            }
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            // BT Feb 11, 2025: prevent switching targets when pass behind wall
            availableTargets.Clear();
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;
                    if (character.transform.root != targetTransform.transform.root && viewableAngle > -50
                        && viewableAngle < 50 && distanceFromTarget < maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position, Color.black, 3f, false);
                            if (hit.transform.gameObject.layer == environmentLayer)
                            {
                                // cannot lock onto target; object in the way
                            }
                            else
                            {
                                availableTargets.Add(character);
                            }
                            // fixed glitch that allowed lock on through walls.  However the following glitch still
                            // remains: if user locked on, then walked behind wall, the lock on remains, and furthermore
                            // the player can switch to other invisible targets with Right Stick (because the other
                            // targets were visible when the lock on occurred).
                        }
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);
                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k].lockOnTransform;
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;

                    if (relativeEnemyPosition.x > 0 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[k].lockOnTransform;
                    }
                    if (relativeEnemyPosition.x < 0 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[k].lockOnTransform;
                    }
                }
            }
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            currentLockOnTarget = null;
            nearestLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);
            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}
