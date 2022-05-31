namespace ServiceManual.ApplicationCore.Entities
{
    public class FactoryDeviceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Year { get; set; }
        public string? Type { get; set; }
        public ICollection<MaintenanceTaskDto>? MaintenanceTasks { get; set; }
    }

    public class MaintenanceTaskDto
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Severity { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public int FactoryDeviceId { get; set; }
    }
}