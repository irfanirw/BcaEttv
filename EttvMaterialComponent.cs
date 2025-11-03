using System;
using Grasshopper.Kernel;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvMaterialComponent : GH_Component
    {
        public EttvMaterialComponent()
          : base("EttvMaterial", "EM",
                 "Create an EttvMaterial (placeholder)",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Material name", GH_ParamAccess.item);
            pManager.AddNumberParameter("Conductivity", "k", "Thermal conductivity (W/m·K)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "t", "Thickness in millimetres (mm)", GH_ParamAccess.item);

            // allow inputs to be empty without producing the yellow/orange missing-input warning
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvMaterial", "M", "EttvMaterial object (not implemented)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = string.Empty;
            double conductivity = 0.0;
            double thicknessMm = 0.0;

            // read inputs if present; don't return on missing inputs (they're optional)
            DA.GetData(0, ref name);
            DA.GetData(1, ref conductivity);
            DA.GetData(2, ref thicknessMm);

            // Placeholder: no runtime warnings emitted when inputs are absent.
            // If no meaningful input was provided, output null (quietly).
            if (string.IsNullOrEmpty(name) && conductivity == 0.0 && thicknessMm == 0.0)
            {
                DA.SetData(0, null);
                return;
            }

            // TODO: construct EttvMaterial (BcaEttvCore.EttvMaterial) and set as output in the future.
            DA.SetData(0, null);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // Load embedded icon from assembly resources
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("BcaEttv.Icons.EttvMaterial.png");
                if (stream == null) return null;
                return new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("a7f3d9e5-1b6a-4c2b-9f58-2a6d5e8b3c77");
    }
}