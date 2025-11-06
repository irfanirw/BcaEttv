using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types; // added for GH_ObjectWrapper / IGH_Goo
using BcaEttvCore;
using System.Reflection;

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
            pManager.AddGenericParameter("EttvMaterials", "M", "List of BcaEttvCore.EttvMaterial objects", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Uvalue", "U", "Calculated U-value (W/m²K)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var raw = new List<object>();
            if (!DA.GetDataList(0, raw) || raw.Count == 0)
            {
                DA.SetData(0, 0.0);
                return;
            }

            var mats = new List<EttvMaterial>();
            foreach (var it in raw)
            {
                object v = it;
                if (v is IGH_Goo goo)
                    v = (goo as GH_ObjectWrapper)?.Value ?? goo.ScriptVariable();

                // direct core type
                if (v is EttvMaterial m)
                {
                    mats.Add(m);
                    continue;
                }

                // same type name from another load context; clone into core type
                var t = v?.GetType();
                if (t?.FullName == "BcaEttvCore.EttvMaterial")
                {
                    var m2 = new EttvMaterial
                    {
                        Name = t.GetProperty("Name")?.GetValue(v)?.ToString() ?? "Unnamed",
                        ThermalConductivity = ToDouble(t.GetProperty("ThermalConductivity")?.GetValue(v)),
                        Thickness = ToDouble(t.GetProperty("Thickness")?.GetValue(v))
                    };
                    mats.Add(m2);
                }
            }

            if (mats.Count == 0)
            {
                DA.SetData(0, 0.0);
                return;
            }

            double u = UvalueCalculator.ComputeUValue(mats);
            DA.SetData(0, u);
        }

        private static double ToDouble(object o) => o == null ? 0.0 : Convert.ToDouble(o);

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