using System.Collections.Generic;
using SystemicOverload.Phase1;
using UnityEngine;

namespace SystemicOverload.Combat
{
    /// <summary>
    /// AttackPoint 주변 OverlapSphere로 근접 피격을 처리합니다.
    /// </summary>
    [RequireComponent(typeof(InputProvider))]
    public sealed class TpsMeleeAttackComponent : MonoBehaviour
    {
        private const string MeleeTriggerParameterName = "MeleeTrig";
        private static readonly int MeleeTriggerHash = Animator.StringToHash(MeleeTriggerParameterName);

        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackPointForwardOffset = 1.2f;
        [SerializeField] private float radius = 1.6f;
        [SerializeField] private float damage = 15.0f;
        [SerializeField] private float cooldown = 0.5f;
        [SerializeField] private LayerMask enemyMask = ~0;
        [SerializeField] private Animator animator;

        private readonly HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        private InputProvider inputProvider;
        private float nextAllowedAttackTime;

        private void Awake()
        {
            inputProvider = GetComponent<InputProvider>();
            EnsureAttackPoint();
        }

        private void OnValidate()
        {
            radius = Mathf.Max(0.1f, radius);
            damage = Mathf.Max(0.0f, damage);
            cooldown = Mathf.Max(0.0f, cooldown);
        }

        private void Update()
        {
            if (!inputProvider.WasMeleePressedThisFrame)
            {
                return;
            }

            if (Time.time < nextAllowedAttackTime)
            {
                return;
            }

            nextAllowedAttackTime = Time.time + cooldown;
            PerformAttack();
            TrySetMeleeTrigger();
        }

        private void PerformAttack()
        {
            EnsureAttackPoint();
            Vector3 center = attackPoint != null ? attackPoint.position : transform.position + transform.forward * attackPointForwardOffset;
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, enemyMask, QueryTriggerInteraction.Ignore);

            damagedTargets.Clear();
            int hitCount = 0;
            for (int index = 0; index < hitColliders.Length; index++)
            {
                Collider targetCollider = hitColliders[index];
                IDamageable targetDamageable = targetCollider.GetComponentInParent<IDamageable>();
                if (targetDamageable == null || !targetDamageable.IsAlive)
                {
                    continue;
                }

                if (!damagedTargets.Add(targetDamageable))
                {
                    continue;
                }

                DamagePayload payload = new DamagePayload
                {
                    Amount = damage,
                    Attacker = transform
                };
                targetDamageable.ApplyDamage(in payload);
                hitCount++;
            }

            Debug.Log($"근접 공격 피격 수: {hitCount}");
        }

        private void TrySetMeleeTrigger()
        {
            if (animator == null)
            {
                return;
            }

            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == MeleeTriggerParameterName && parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.SetTrigger(MeleeTriggerHash);
                    return;
                }
            }
        }

        private void EnsureAttackPoint()
        {
            if (attackPoint != null)
            {
                return;
            }

            Transform existingChild = transform.Find("AttackPoint");
            if (existingChild != null)
            {
                attackPoint = existingChild;
                return;
            }

            GameObject attackPointObject = new GameObject("AttackPoint");
            attackPointObject.transform.SetParent(transform);
            attackPointObject.transform.localPosition = new Vector3(0.0f, 1.0f, attackPointForwardOffset);
            attackPointObject.transform.localRotation = Quaternion.identity;
            attackPoint = attackPointObject.transform;
        }

        private void OnDrawGizmosSelected()
        {
            Transform targetPoint = attackPoint != null ? attackPoint : transform;
            Vector3 center = attackPoint != null
                ? targetPoint.position
                : transform.position + transform.forward * attackPointForwardOffset;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}
