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

            // If no meaningful input was provided, output null (quietly).
            if (string.IsNullOrEmpty(name) && conductivity == 0.0 && thicknessMm == 0.0)
            {
                DA.SetData(0, null);
                return;
            }

            // Resolve EttvMaterial from BcaEttvCore.dll explicitly (avoid any local type)
            System.Reflection.Assembly coreAsm = null;
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(a.GetName().Name, "BcaEttvCore", System.StringComparison.Ordinal))
                {
                    coreAsm = a;
                    break;
                }
            }
            if (coreAsm == null)
            {
                try { coreAsm = System.Reflection.Assembly.Load("BcaEttvCore"); }
                catch { DA.SetData(0, null); return; }
            }

            var materialType = coreAsm.GetType("BcaEttvCore.EttvMaterial", throwOnError: false);
            if (materialType == null)
            {
                DA.SetData(0, null);
                return;
            }

            var material = System.Activator.CreateInstance(materialType);

            // Set properties via reflection
            TrySet(material, "Name", name);
            TrySet(material, "ThermalConductivity", conductivity);
            TrySet(material, "Thickness", thicknessMm / 1000.0); // mm -> m

            DA.SetData(0, material);

            // Local helper to set a property if it exists and is writable
            void TrySet(object target, string prop, object value)
            {
                var pi = target.GetType().GetProperty(prop, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (pi == null || !pi.CanWrite) return;

                var destType = pi.PropertyType;
                var underlying = System.Nullable.GetUnderlyingType(destType);
                var targetType = underlying ?? destType;

                try
                {
                    if (value != null && targetType != value.GetType())
                    {
                        if (value is System.IConvertible && typeof(System.IConvertible).IsAssignableFrom(targetType))
                        {
                            value = System.Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
                catch { /* ignore conversion errors */ }

                pi.SetValue(target, value);
            }
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
#pragma warning disable CA1416 // Validate platform compatibility
                return new System.Drawing.Bitmap(stream);
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }

        public override Guid ComponentGuid => new Guid("a7f3d9e5-1b6a-4c2b-9f58-2a6d5e8b3c77");
    }
}