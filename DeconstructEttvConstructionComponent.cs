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
            pManager.AddTextParameter("Text", "T", "Readable construction details", GH_ParamAccess.item);          // index 0
            pManager.AddNumberParameter("Uvalue", "U", "U-value (W/m²K)", GH_ParamAccess.item);                    // index 1
            pManager.AddNumberParameter("ScValue", "SC", "Solar control value (ScTotal)", GH_ParamAccess.item);    // index 2
            pManager.AddGenericParameter("Materials", "M", "List of EttvMaterial", GH_ParamAccess.list);           // index 3
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object raw = null;
            DA.GetData(0, ref raw);

            if (raw is IGH_Goo goo)
                raw = (goo as GH_ObjectWrapper)?.Value ?? goo.ScriptVariable();

            string result = string.Empty;
            var mats = new List<EttvMaterial>();
            double uValue = 0.0;
            double scValue = 0.0;
            bool scIsEmpty = false;

            if (raw is EttvConstruction cons && raw.GetType().Assembly == typeof(EttvConstruction).Assembly)
            {
                result = EttvConstructionDeconstructor.ToText(cons);
                uValue = cons.Uvalue;
                if (cons.EttvMaterials != null)
                    mats.AddRange(cons.EttvMaterials);

                if (cons is EttvFenestrationConstruction fen)
                {
                    scValue = fen.ScTotal;
                    if (scValue == 0.0)
                        scValue = fen.Sc1 * (fen.Sc2 == 0 ? 1.0 : fen.Sc2);
                }
                else if (cons is EttvOpaqueConstruction)
                {
                    scIsEmpty = true;
                }
                else
                {
                    var pi = cons.GetType().GetProperty("ScTotal", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (pi != null)
                    {
                        var v = pi.GetValue(cons);
                        if (v != null)
                            scValue = Convert.ToDouble(v);
                    }
                }
            }
            else if (raw == null)
            {
                result = string.Empty;
            }
            else
            {
                result = "EttvConstruction: invalid input type";
            }

            DA.SetData(0, result);
            DA.SetData(1, uValue);
            if (scIsEmpty) DA.SetData(2, null);
            else DA.SetData(2, scValue);
            DA.SetDataList(3, mats);
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