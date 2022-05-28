namespace ServiceManual.ApplicationCore.Entities
{
    public class FactoryDeviceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Year { get; set; }
        public string? Type { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Severity { get; set; }
    }
}