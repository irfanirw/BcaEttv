using System;
using System.Collections.Generic;
using System.IO; // added
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvModelComponent : GH_Component
    {
        public EttvModelComponent()
          : base("EttvModel", "EM",
                 "Create an ETTV Model from surfaces and export settings",
                 "BcaEttv", "Model Setup")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ProjectName", "PN", "Override EttvModel.ProjectName", GH_ParamAccess.item);
            pManager.AddTextParameter("Version", "V", "Override EttvModel.Version", GH_ParamAccess.item);
            pManager.AddGenericParameter("EttvSurfaces", "S", "List of EttvSurface objects", GH_ParamAccess.list);
            pManager.AddBooleanParameter("WriteJson", "W", "Write EttvModel JSON file to disk", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Directory", "D", "Output directory (ignored; exports next to .gh file)", GH_ParamAccess.item);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Operation status and messages", GH_ParamAccess.item);
            pManager.AddGenericParameter("EttvModel", "M", "EttvModel object", GH_ParamAccess.item);
            pManager.AddTextParameter("FilePath", "F", "Path to exported JSON file", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string projectName = null;
            string version = null;
            var rawSurfaces = new List<object>();
            bool writeJson = false;
            string directory = null;

            DA.GetData(0, ref projectName);
            DA.GetData(1, ref version);
            DA.GetDataList(2, rawSurfaces);
            DA.GetData(3, ref writeJson);
            DA.GetData(4, ref directory);

            // Extract valid EttvSurface objects
            var surfaces = new List<EttvSurface>();
            foreach (var item in rawSurfaces)
            {
                object v = item;
                if (v is IGH_Goo goo)
                    v = (goo as GH_ObjectWrapper)?.Value ?? goo.ScriptVariable();

                if (v is EttvSurface s && v.GetType().Assembly == typeof(EttvSurface).Assembly)
                    surfaces.Add(s);
            }

            if (surfaces.Count == 0)
            {
                DA.SetData(0, "No valid EttvSurface objects provided.");
                DA.SetData(1, null);
                DA.SetData(2, string.Empty);
                return;
            }

            // Create EttvModel
            var model = new EttvModel(surfaces);

            // Override auto defaults if user supplied values
            if (!string.IsNullOrWhiteSpace(projectName))
                model.ProjectName = projectName;
            if (!string.IsNullOrWhiteSpace(version))
                model.Version = version;

            string status = $"EttvModel created: {model.ProjectName} v{model.Version}\n";
            status += $"Surfaces: {model.Surfaces.Count}";

            string filePath = string.Empty;

            if (writeJson)
            {
                try
                {
                    // Always export next to the current .gh file
                    var ghDoc = OnPingDocument();
                    string ghDir = null;
                    if (ghDoc != null && !string.IsNullOrWhiteSpace(ghDoc.FilePath))
                        ghDir = Path.GetDirectoryName(ghDoc.FilePath);
                    if (string.IsNullOrWhiteSpace(ghDir))
                    {
                        // Fallback when the GH file is not saved yet
                        ghDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Grasshopper file is not saved. Exporting to Documents folder.");
                    }
                    directory = ghDir; // override any user-provided directory
                    Directory.CreateDirectory(directory);
                    filePath = BuildExportPath(model, directory);
                    EttvModelExporter.ExportToJson(model, filePath);
                    status += $"\n✓ JSON exported: {filePath}";
                }
                catch (Exception ex)
                {
                    status += $"\n✗ JSON export failed: {ex.Message}";
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"JSON export failed: {ex.Message}");
                }
            }

            DA.SetData(0, status);
            DA.SetData(1, model);
            DA.SetData(2, filePath);
        }

        private static string BuildExportPath(EttvModel model, string directory)
        {
            var project = SanitizeFileNameSegment(model?.ProjectName, "EttvModel");
            var version = SanitizeFileNameSegment(model?.Version, "v1");
            var fileName = $"{project}_{version}.json";
            return Path.Combine(directory ?? string.Empty, fileName);
        }

        private static string SanitizeFileNameSegment(string value, string fallback)
        {
            var candidate = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(candidate
                .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                .ToArray())
                .Trim('_');

            return string.IsNullOrWhiteSpace(sanitized) ? fallback : sanitized;
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.EttvModel.png");
#pragma warning disable CA1416
                return stream is null ? null : new System.Drawing.Bitmap(stream);
#pragma warning restore CA1416
            }
        }

        public override Guid ComponentGuid => new Guid("BBA8E894-9193-4563-8A4F-E4D197778691");
    }
}
