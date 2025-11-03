using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvUValueComponent : GH_Component
    {
        public EttvUValueComponent()
          : base("EttvUValueCalculator", "EU",
                 "Calculate U-value from a list of materials",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvMaterials", "M", "List of EttvMaterial objects", GH_ParamAccess.list);
            // allow the input to be empty without producing a yellow/orange warning
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Uvalue", "U", "Calculated U-value (W/m²K)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // If no input is provided, return a default value quietly (no warning).
            var materials = new List<EttvMaterial>();
            if (!DA.GetDataList(0, materials) || materials.Count == 0)
            {
                // output default U-value (or leave as null/0). No warning emitted.
                DA.SetData(0, 0.0);
                return;
            }

            // TODO: Implement U-value calculation from material layers
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "U-value calculation not implemented yet");
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.EttvUValue.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("c5e2d1f8-9a3b-4d7e-b6c5-1f2a3b4c5d6e");
    }
}