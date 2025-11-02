using System;
using Grasshopper.Kernel;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvOpaqueConstruction : GH_Component
    {
        public EttvOpaqueConstruction()
          : base("EttvOpaqueConstruction", "EOC",
                 "Create an EttvConstruction (opaque) from name and U-value",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // added Id as first input (tracking label)
            pManager.AddTextParameter("Id", "Id", "Tracking label / identifier for reports", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Construction name", GH_ParamAccess.item);
            pManager.AddNumberParameter("Uvalue", "U", "Thermal transmittance (U-value, W/m²K)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvConstruction object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // TODO: implement EttvConstruction creation here.
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "EttvOpaqueConstruction: SolveInstance not implemented.");
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // Load embedded icon from assembly resources
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("BcaEttv.Icons.EttvConstruction.png");
                if (stream == null) return null;
                return new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("b8a3c2f2-d4b3-4c7c-9f6d-5d6e5b7c8a11");
    }
}