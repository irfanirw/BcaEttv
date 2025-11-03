using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvSurfaceComponent : GH_Component
    {
        public EttvSurfaceComponent()
          : base("EttvSurfaceComponent", "ES",
                 "Create a list of EttvSurface from meshes and a construction",
                 "BcaEttv", "Geometry & Inputs")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // list of Rhino meshes
            pManager.AddMeshParameter("Meshes", "M", "Input Rhino meshes", GH_ParamAccess.list);
            // EttvConstruction (from BcaEttvCore)
            pManager.AddGenericParameter("EttvConstruction", "C", "EttvConstruction object", GH_ParamAccess.item);

            // allow inputs to be empty without producing the yellow/orange missing-input warning
            pManager[0].Optional = true;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // list of EttvSurface objects (not implemented yet)
            pManager.AddGenericParameter("EttvSurfaces", "S", "List of EttvSurface objects (not implemented)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Placeholder for future implementation.
            // Read inputs quietly (they are optional) and return an empty list for now.

            var meshes = new List<Mesh>();
            EttvConstruction construction = null;

            // Try to get inputs if provided; missing inputs won't trigger warnings because params are optional.
            DA.GetDataList(0, meshes);
            DA.GetData(1, ref construction);

            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "EttvSurfaceComponent: SolveInstance not implemented.");

            // Output an empty list until implementation is provided.
            DA.SetDataList(0, new List<object>());
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                string fileName = "EttvSurface.png";
                // find resource name robustly (case-insensitive)
                var resourceName = Array.Find(asm.GetManifestResourceNames(),
                    n => n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));
                if (resourceName is null)
                    resourceName = Array.Find(asm.GetManifestResourceNames(),
                        n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
                if (resourceName is null) return null;

                using var stream = asm.GetManifestResourceStream(resourceName);
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("e3b1c9d2-4f6a-4b2e-9d1f-0a1b2c3d4e5f");
    }
}