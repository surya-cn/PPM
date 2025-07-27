using System;

namespace PowerPlanViewer
{
    public class PowerPlan
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return $"{(IsActive ? "[ACTIVE] " : "")}{Name}";
        }
    }
}
