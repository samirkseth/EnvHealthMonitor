namespace Health.Models
{
    public class EnvironmentSummary
    {
        public string EnvName { get; set; }
        public HealthStatus Health { get; set; }
    }
}