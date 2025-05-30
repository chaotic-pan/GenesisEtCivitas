using UnityEngine;
using UnityEngine.Serialization;

namespace Animals
{
    public abstract class Animal : MonoBehaviour
    {
        private Animator _animator;

        protected virtual void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        protected virtual void Idle()
        {
            Debug.Log($"Animal {name} does not idle!");
        }
        
        protected virtual void Walk()
        {
            Debug.Log($"Animal {name} does not walk!");
        }
        
        protected virtual void Attack()
        {
            Debug.Log($"Animal {name} does not attack!");
        }
        
        protected virtual void Die()
        {
            Debug.Log($"Animal {name} does not die!");
        }
        
        protected void PlayAnimation(AnimalState state)
        {
            _animator.SetInteger("State", (int)state);
        }
        
        protected enum AnimalState {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Die = 3
        }
    }
}
