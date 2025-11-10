using System.Collections.Generic;
using System.Linq;

namespace BcaEttvCore
{
    public class EttvModel
    {
        private static int _projectCounter = 0;
        private static int _versionCounter = 0;

        public string ProjectName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool Reordered { get; set; } = false;

        private List<EttvSurface> _surfaces = new();
        public List<EttvSurface> Surfaces
        {
            get => _surfaces;
            set
            {
                _surfaces = value ?? new List<EttvSurface>();
                RenumberSurfaceIds();
            }
        }

        public List<List<EttvSurface>> EttvSurfaceOrder { get; set; } = new();
        public EttvComputationResult ComputationResult { get; set; }

        public EttvModel() { }

        public EttvModel(List<EttvSurface> surfaces)
        {
            Surfaces = surfaces ?? new List<EttvSurface>();
            ProjectName = $"EttvProject_{_projectCounter++}";
            Version = (_versionCounter++).ToString();
            Reordered = false;
            RenumberSurfaceIds();
        }

        /// <summary>
        /// Cluster surfaces based on EttvOrientation.Name into nested lists of EttvSurface.
        /// Does not mutate the Surfaces list order. Sets Reordered = true.
        /// </summary>
        public void ReorderEttvSurfaces()
        {
            if (Surfaces == null || Surfaces.Count == 0)
            {
                EttvSurfaceOrder = new List<List<EttvSurface>>();
                Reordered = true;
                return;
            }

            var orderMap = new Dictionary<string, int>
            {
                ["North"] = 0,
                ["NorthEast"] = 1,
                ["East"] = 2,
                ["SouthEast"] = 3,
                ["South"] = 4,
                ["SouthWest"] = 5,
                ["West"] = 6,
                ["NorthWest"] = 7,
                ["Roof"] = 8,
                ["Floor"] = 9,
                ["Unknown"] = 10
            };

            var grouped = Surfaces
                .Select(s => new { s, key = s?.Orientation?.Name ?? "Unknown" })
                .GroupBy(x => x.key)
                .OrderBy(g => orderMap.TryGetValue(g.Key, out var k) ? k : int.MaxValue)
                .ThenBy(g => g.Key);

            EttvSurfaceOrder = grouped
                .Select(g => g.Select(x => x.s).ToList())
                .ToList();

            Reordered = true;
        }

        private void RenumberSurfaceIds()
        {
            if (_surfaces == null) return;
            for (int i = 0; i < _surfaces.Count; i++)
            {
                if (_surfaces[i] != null)
                    _surfaces[i].Id = i;
            }
        }
    }
}