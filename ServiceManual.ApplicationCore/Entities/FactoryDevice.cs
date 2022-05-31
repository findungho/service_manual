using System;
using System.Collections.Generic;

namespace ServiceManual.ApplicationCore.Entities
{
    public class FactoryDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
        public ICollection<MaintenanceTask> MaintenanceTasks { get; set; }
    }

    public class MaintenanceTask
    {
        public int Id { get; set; }
        public Status Status { get; set; }
        public Severity Severity { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int FactoryDeviceId { get; set; }
    }

    public enum Severity
    {
        Critical,
        Important,
        Unimportant
    }

    public enum Status
    {
        Open,
        Closed
    }
}