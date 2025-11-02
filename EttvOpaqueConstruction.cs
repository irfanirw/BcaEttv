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
            pManager.AddTextParameter("Name", "N", "Construction name", GH_ParamAccess.item);
            pManager.AddNumberParameter("Uvalue", "U", "Thermal transmittance (U-value, W/m²K)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvConstruction object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = string.Empty;
            double uvalue = 0.0;

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref uvalue)) return;

            if (uvalue < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "U-value must be >= 0");
                return;
            }

            var cons = new EttvConstruction
            {
                Name = name,
                UValue = uvalue
            };

            DA.SetData(0, cons);
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