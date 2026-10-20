public class DomainScore
{
    public PreparednessDomain domain;
    public int completed;
    public int total;
    public float earned;
    public float possible;

    public DomainScore(PreparednessDomain domain)
    {
        this.domain = domain;
    }

    public float Percent
    {
        get { return possible > 0f ? earned * 100f / possible : 0f; }
    }
}
