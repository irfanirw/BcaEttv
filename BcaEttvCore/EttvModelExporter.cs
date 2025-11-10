using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BcaEttvCore
{
    public class EttvModelExporter
    {
        public EttvModel EttvModel { get; set; }
        public string Directory { get; set; } = string.Empty;

        public EttvModelExporter() { }

        public EttvModelExporter(EttvModel model)
        {
            EttvModel = model;
        }

        /// <summary>
        /// Export EttvModel to JSON excluding any Rhino geometry objects.
        /// Geometry (Mesh, Brep, etc.) is not written. Orientation exports only Name.
        /// Construction exports basic identifiers and U-value / SC where available.
        /// </summary>
        public string ExportEttv(string directory = null)
        {
            if (EttvModel == null)
                throw new InvalidOperationException("EttvModel is not set.");

            // Choose target directory
            string targetDir = string.IsNullOrWhiteSpace(directory)
                ? System.IO.Directory.GetCurrentDirectory()
                : directory;

            if (!System.IO.Directory.Exists(targetDir))
                System.IO.Directory.CreateDirectory(targetDir);

            string safeProjectName = string.IsNullOrWhiteSpace(EttvModel.ProjectName)
                ? "EttvModel"
                : EttvModel.ProjectName.Replace(" ", "_");

            string fileName = $"{safeProjectName}_v{EttvModel.Version}.json";
            string fullPath = Path.Combine(targetDir, fileName);

            // Build lightweight export DTO (no Rhino geometry)
            var exportObj = new EttvModelExportDto
            {
                ProjectName = EttvModel.ProjectName,
                Version = EttvModel.Version,
                Reordered = EttvModel.Reordered,
                // Map nested lists of EttvSurface -> nested lists of surface Ids for JSON
                EttvSurfaceOrder = EttvModel.EttvSurfaceOrder?
                    .Select(inner => inner?.Select(s => s?.Id ?? -1).ToList() ?? new List<int>())
                    .ToList() ?? new(),
                Surfaces = EttvModel.Surfaces?
                    .Select(s =>
                    {
                        var fen = s.Construction as EttvFenestrationConstruction;
                        double? scTotal = null;
                        if (fen != null)
                            scTotal = fen.ScTotal != 0 ? fen.ScTotal : fen.Sc1 * (fen.Sc2 == 0 ? 1.0 : fen.Sc2);

                        return new EttvSurfaceExportDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Type = s.Type,
                            Orientation = s.Orientation?.Name,
                            ConstructionId = s.Construction != null ? s.Construction.Id?.ToString() : null,
                            ConstructionName = s.Construction?.Name,
                            Uvalue = s.Construction?.Uvalue,
                            ScTotal = scTotal,
                            MaterialCount = s.Construction?.EttvMaterials?.Count ?? 0
                        };
                    })
                    .ToList() ?? new()
            };

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            string jsonString = JsonConvert.SerializeObject(exportObj, settings);
            File.WriteAllText(fullPath, jsonString);
            Directory = fullPath;
            return fullPath;
        }

        // DTO classes (no Rhino.Geometry types)
        private class EttvModelExportDto
        {
            public string ProjectName { get; set; }
            public string Version { get; set; }
            public bool Reordered { get; set; }
            public List<List<int>> EttvSurfaceOrder { get; set; }
            public List<EttvSurfaceExportDto> Surfaces { get; set; }
        }

        private class EttvSurfaceExportDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Orientation { get; set; }
            public string ConstructionId { get; set; }
            public string ConstructionName { get; set; }
            public double? Uvalue { get; set; }
            public double? ScTotal { get; set; }
            public int MaterialCount { get; set; }
        }
    }
}
