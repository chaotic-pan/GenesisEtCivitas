namespace Animals
{
    public class Fish : Animal
    {
        private void Start()
        {
            Walk();   
            Idle();
        }

        protected override void Walk()
        {
            PlayAnimation(AnimalState.Walk);
        } 
    }
}
