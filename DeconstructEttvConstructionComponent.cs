using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using BcaEttvCore;

namespace BcaEttv
{
    public class DeconstructEttvConstructionComponent : GH_Component
    {
        public DeconstructEttvConstructionComponent()
          : base("DeconstructEttvConstruction", "DEC",
                 "Deconstruct an EttvConstruction (Opaque or Fenestration) to a readable text",
                 "BcaEttv", "Utilities")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvOpaqueConstruction or EttvFenestrationConstruction", GH_ParamAccess.item);
            pManager[0].Optional = true; // avoid yellow warning when nothing connected
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Readable construction details", GH_ParamAccess.item);
            pManager.AddGenericParameter("Materials", "M", "List of EttvMaterial", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object raw = null;
            DA.GetData(0, ref raw);

            // unwrap GH goo
            if (raw is IGH_Goo goo)
                raw = (goo as GH_ObjectWrapper)?.Value ?? goo.ScriptVariable();

            string result = string.Empty;
            var mats = new List<EttvMaterial>();

            if (raw is EttvConstruction cons && raw.GetType().Assembly == typeof(EttvConstruction).Assembly)
            {
                // Use core helper
                result = DeconstructEttvConstruction.ToText(cons);
                if (cons.EttvMaterials != null)
                    mats.AddRange(cons.EttvMaterials);
            }
            else if (raw == null)
            {
                result = string.Empty; // quiet when nothing provided
            }
            else
            {
                // Unsupported type provided
                result = "EttvConstruction: invalid input type";
            }

            DA.SetData(0, result);
            DA.SetDataList(1, mats);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.DeconstructEttvConstruction.png");
#pragma warning disable CA1416
                return stream == null ? null : new System.Drawing.Bitmap(stream);
#pragma warning restore CA1416
            }
        }

        public override Guid ComponentGuid => new Guid("3c0d5b2a-8c7e-4a77-9d8c-2b1a6f4e5d7a");
    }
}