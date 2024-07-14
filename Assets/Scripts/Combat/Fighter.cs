using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Control;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        // [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        // [SerializeField] float weaponDamage = 5f;
        // [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        // [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] Weapon defaultWeapon = null;


        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        private void Start()
        {
            EquipWeapon(defaultWeapon);
        }

        
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;

            if(target.IsDead()) return;

            if(!GetsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            // Instantiate(weaponPrefab, rightHandTransform);
            // if(weapon == null) return;
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            // animator.runtimeAnimatorController = weaponOverride;
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if(timeSinceLastAttack  > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
                
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }


        void Hit()
        {
            // Health healthComponent = target.GetComponent<Health>();
            // healthComponent.TakeDamage(weaponDamage);
            if(target == null) return;

            if(currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
            }
            else
            {
                target.TakeDamage(currentWeapon.GetDamage());
                
            }

            // target.TakeDamage(weaponDamage);
        }

        void Shoot()
        {
            Hit();
        }
        private bool GetsInRange()
        {
            // return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            // print("hit");
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");

        }

        
    }

}
