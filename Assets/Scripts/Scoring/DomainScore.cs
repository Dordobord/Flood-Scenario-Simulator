public class DomainScore
{
    public PreparednessDomain domain;
    public int completed;
    public int total;

    public DomainScore(PreparednessDomain domain)
    {
        this.domain = domain;
    }

    public float Ratio
    {
        get { return total > 0 ? (float)completed / total : 0f; }
    }
}
