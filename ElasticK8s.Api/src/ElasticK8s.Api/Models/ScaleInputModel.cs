namespace ElasticK8s.Api.Models
{
    public class ScaleInputModel
    {
        public string Deployment { get; set; }
        public string Namespace { get; set; }
        public int Scale { get; set; }
    }
}