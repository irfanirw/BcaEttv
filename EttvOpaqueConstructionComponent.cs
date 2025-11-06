using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using BcaEttvCore;

namespace BcaEttv
{
    // Rename to avoid clashing with core class BcaEttvCore.EttvOpaqueConstruction
    public class EttvOpaqueConstructionComponent : GH_Component
    {
        public EttvOpaqueConstructionComponent()
          : base("EttvOpaqueConstruction", "EOC",
                 "Create an EttvConstruction (opaque) from Id, Name and Materials",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Id", "ID", "Construction identifier (string)", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Construction name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Materials", "M", "List of BcaEttvCore.EttvMaterial", GH_ParamAccess.list);

            // keep inputs optional to avoid yellow warnings
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Output base class (EttvConstruction). Instance will be EttvOpaqueConstruction.
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvConstruction (opaque) object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string id = null;
            string name = null;
            var rawMaterials = new List<object>();

            DA.GetData(0, ref id);
            DA.GetData(1, ref name);
            DA.GetDataList(2, rawMaterials);

            // Collect only BcaEttvCore.EttvMaterial
            var materials = new List<EttvMaterial>();
            var coreMatType = typeof(EttvMaterial);

            foreach (var item in rawMaterials)
            {
                object v = item;
                if (v is IGH_Goo goo)
                    v = (goo as GH_ObjectWrapper)?.Value ?? goo.ScriptVariable();

                if (v is EttvMaterial m && v.GetType().Assembly == coreMatType.Assembly)
                    materials.Add(m);
            }

            // If nothing provided, return quietly
            bool anyProvided = !string.IsNullOrWhiteSpace(id) || !string.IsNullOrEmpty(name) || materials.Count > 0;
            if (!anyProvided)
            {
                DA.SetData(0, null);
                return;
            }

            // Compute U-value if materials are provided
            double u = materials.Count > 0 ? UvalueCalculator.ComputeUValue(materials) : 0.0;

            // Build opaque construction
            var opaque = new EttvOpaqueConstruction
            {
                Id = id ?? string.Empty,
                Name = name ?? string.Empty,
                EttvMaterials = materials,
                Uvalue = u
            };

            // Output as base class
            DA.SetData(0, (EttvConstruction)opaque);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // Load embedded icon from assembly resources
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("BcaEttv.Icons.EttvConstruction.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("8b6b8a2f-3f8f-4f7a-bf2c-9c5c2e7a4c11");
    }
}