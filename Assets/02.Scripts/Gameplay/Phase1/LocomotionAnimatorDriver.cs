using UnityEngine;

namespace SystemicOverload.Phase1
{
    /// <summary>
    /// Movement/CharacterController 상태를 Animator 파라미터로 전달합니다. 클립은 Animator Controller에서 배치합니다.
    /// StarterAssets TPS Controller 파라미터(Grounded, FreeFall, MotionSpeed)와 커스텀 파라미터(IsGrounded, VerticalVelocity) 모두 지원합니다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(50)]
    public sealed class LocomotionAnimatorDriver : MonoBehaviour
    {
        // 커스텀
        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int IsGroundedId = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalVelocityId = Animator.StringToHash("VerticalVelocity");
        // StarterAssets TPS Controller
        private static readonly int GroundedId = Animator.StringToHash("Grounded");
        private static readonly int FreeFallId = Animator.StringToHash("FreeFall");
        private static readonly int MotionSpeedId = Animator.StringToHash("MotionSpeed");

        [SerializeField] private MovementComponent movementComponent;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float speedDampTime = 0.08f;
        [SerializeField] private float freeFallVelocityThreshold = -1.5f;

        private Animator animator;
        private bool hasSpeed;
        private bool hasIsGrounded;
        private bool hasVerticalVelocity;
        private bool hasGrounded;
        private bool hasFreeFall;
        private bool hasMotionSpeed;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movementComponent ??= GetComponent<MovementComponent>();
            characterController ??= GetComponent<CharacterController>();
            CacheParameterAvailability();
        }

        private void OnEnable()
        {
            CacheParameterAvailability();
        }

        private void OnValidate()
        {
            speedDampTime = Mathf.Max(0.0f, speedDampTime);
        }

        private void Update()
        {
            if (animator == null)
            {
                return;
            }

            float normalizedSpeed = movementComponent != null ? movementComponent.NormalizedPlanarSpeed : 0.0f;
            bool grounded = characterController != null && characterController.isGrounded;
            float verticalVelocity = movementComponent != null ? movementComponent.VerticalVelocity : 0.0f;

            if (hasSpeed)
            {
                animator.SetFloat(SpeedId, normalizedSpeed, speedDampTime, Time.deltaTime);
            }

            if (hasIsGrounded)
            {
                animator.SetBool(IsGroundedId, grounded);
            }

            if (hasVerticalVelocity)
            {
                animator.SetFloat(VerticalVelocityId, verticalVelocity);
            }

            if (hasGrounded)
            {
                animator.SetBool(GroundedId, grounded);
            }

            if (hasFreeFall)
            {
                animator.SetBool(FreeFallId, !grounded && verticalVelocity < freeFallVelocityThreshold);
            }

            if (hasMotionSpeed)
            {
                animator.SetFloat(MotionSpeedId, normalizedSpeed, speedDampTime, Time.deltaTime);
            }
        }

        private void CacheParameterAvailability()
        {
            hasSpeed = false;
            hasIsGrounded = false;
            hasVerticalVelocity = false;
            hasGrounded = false;
            hasFreeFall = false;
            hasMotionSpeed = false;

            if (animator == null)
            {
                return;
            }

            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Float && p.nameHash == SpeedId) hasSpeed = true;
                if (p.type == AnimatorControllerParameterType.Bool && p.nameHash == IsGroundedId) hasIsGrounded = true;
                if (p.type == AnimatorControllerParameterType.Float && p.nameHash == VerticalVelocityId) hasVerticalVelocity = true;
                if (p.type == AnimatorControllerParameterType.Bool && p.nameHash == GroundedId) hasGrounded = true;
                if (p.type == AnimatorControllerParameterType.Bool && p.nameHash == FreeFallId) hasFreeFall = true;
                if (p.type == AnimatorControllerParameterType.Float && p.nameHash == MotionSpeedId) hasMotionSpeed = true;
            }
        }
    }
}
