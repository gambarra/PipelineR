namespace PipelineR
{
    public class CacheSettings
    {
        public int TTLInMinutes { get; set; } = 1;
        public string ConnectionString { get; set; }
        public string Preffix { get; set; }
    }
}
