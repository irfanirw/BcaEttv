using System.Collections.Generic;

namespace BcaEttvCore
{
    public class EttvModel
    {
        private static int _projectCounter = 0;
        private static int _versionCounter = 0;

        public string ProjectName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool Reordered { get; set; } = false;
        public List<EttvSurface> Surfaces { get; set; } = new();
        public double? EttvValue { get; set; }

        public EttvModel() { }

        public EttvModel(List<EttvSurface> surfaces)
        {
            Surfaces = surfaces ?? new List<EttvSurface>();
            ProjectName = $"EttvProject_{_projectCounter++}";
            Version = (_versionCounter++).ToString();
            Reordered = false;
        }
    }
}