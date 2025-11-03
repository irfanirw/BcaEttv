using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using BcaEttvCore; // added so we can reference EttvConstruction

namespace BcaEttv
{
    public class EttvFenestrationConstruction2Component : GH_Component
    {
        public EttvFenestrationConstruction2Component()
          : base("EttvFenestrationConstruction", "EFC2",
                 "Create an ETTV Fenestration Construction",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Id", "ID", "Tracking identifier", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Construction name", GH_ParamAccess.item);
            pManager.AddNumberParameter("Uvalue", "U", "U-value (W/m²K)", GH_ParamAccess.item);
            pManager.AddNumberParameter("SC1", "SC1", "Solar coefficient 1 value", GH_ParamAccess.item);

            // allow inputs to be empty without producing the yellow/orange missing-input warning
            for (int i = 0; i < pManager.ParamCount; i++)
                Params.Input[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvConstruction object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Read inputs if present; keep defaults when not provided.
            string id = null;
            string name = null;
            double uvalue = double.NaN;
            double sc1 = double.NaN;

            DA.GetData(0, ref id);    // optional
            DA.GetData(1, ref name);  // optional
            DA.GetData(2, ref uvalue);// optional
            DA.GetData(3, ref sc1);   // optional

            // No warnings produced when inputs are missing.
            // Placeholder: creation logic to be implemented later.
            DA.SetData(0, null);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.EttvFenestrationConstruction.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("f7e8d9c0-b1a2-4c3d-9e4f-5a6b7c8d9e0f");
    }
}