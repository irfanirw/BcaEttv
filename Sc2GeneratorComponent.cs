using System;
using Grasshopper.Kernel;

namespace BcaEttv
{
    public class Sc2GeneratorComponent : GH_Component
    {
        public Sc2GeneratorComponent()
          : base("Sc2Generator", "SC2",
                 "Generate SC2 value for ETTV calculation",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ShadingType", "T", "Shading type (vertical/horizontal)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth", "D", "Shading device depth (m)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Projection", "P", "Shading device projection (m)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Opening height (m)", GH_ParamAccess.item);

            // allow inputs to be empty without producing the yellow/orange missing-input warning
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Sc2", "SC2", "SC2 value for ETTV calculation", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string shadingType = string.Empty;
            double depth = double.NaN;
            double projection = double.NaN;
            double height = double.NaN;

            // read inputs if present (optional); missing inputs won't produce warnings
            DA.GetData(0, ref shadingType);
            DA.GetData(1, ref depth);
            DA.GetData(2, ref projection);
            DA.GetData(3, ref height);

            // if nothing was provided, return a quiet default (no warning)
            bool anyProvided =
                !string.IsNullOrEmpty(shadingType) ||
                !double.IsNaN(depth) ||
                !double.IsNaN(projection) ||
                !double.IsNaN(height);

            if (!anyProvided)
            {
                DA.SetData(0, 0.0); // quiet default
                return;
            }

            // TODO: Implement SC2 calculation logic
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "SC2 calculation not implemented yet");
            DA.SetData(0, 0.0);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.Sc2Generator.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("d4e5f6a7-b8c9-4d0e-a1b2-c3d4e5f6a7b8");
    }
}