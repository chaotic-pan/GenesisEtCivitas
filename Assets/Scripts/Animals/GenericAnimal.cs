using System;

namespace Animals
{
    public class GenericAnimal : Animal
    {
        private void Start()
        {
            Idle();   
        }

        protected override void Idle()
        {
            PlayAnimation(AnimalState.Idle);
        } 
        
        protected override void Walk()
        {
            PlayAnimation(AnimalState.Walk);
        } 
        
        protected override void Attack()
        {
            PlayAnimation(AnimalState.Attack);
        } 
        
        protected override void Die()
        {
            PlayAnimation(AnimalState.Die);
        } 
    }
}
