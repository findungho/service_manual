using System;

namespace ServiceManual.ApplicationCore.Entities
{
    public class FactoryDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public Severity Severity { get; set; }
    }

    public enum Severity
    {
        Critical,
        Important,
        Unimportant
    }
}