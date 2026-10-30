public class DomainScore
{
    public PreparednessDomain domain;
    public float maxPoints;
    public float earnedWeight;
    public float totalWeight;

    public DomainScore(PreparednessDomain domain, float maxPoints)
    {
        this.domain = domain;
        this.maxPoints = maxPoints;
    }

    public float Points
    {
        get
        {
            if (totalWeight <= 0f) return 0f;
            return earnedWeight / totalWeight * maxPoints;
        }
    }
}
