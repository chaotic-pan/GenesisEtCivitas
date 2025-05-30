using UnityEngine.Events;

public static class GameEvents
{
    public static readonly InfluencePointsEvents InfluencePoints = new ();
    
    public class InfluencePointsEvents
    {
        public UnityAction<int> GainInfluencePoints;
    }
}