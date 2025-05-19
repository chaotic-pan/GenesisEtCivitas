namespace Animals
{
    public class Fish : Animal
    {
        private void Start()
        {
            Walk();   
        }

        protected override void Walk()
        {
            PlayAnimation(AnimalState.Walk);
        } 
    }
}
