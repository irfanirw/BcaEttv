using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using BcaEttvCore;

namespace BcaEttv
{
    public class EttvModelComponent : GH_Component
    {
        public EttvModelComponent()
          : base("EttvModel", "EM",
                 "Create an ETTV Model from surfaces and export settings",
                 "BcaEttv", "Model Setup")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvSurfaces", "S", "List of EttvSurface objects", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Reorder", "R", "Reorder surfaces for visualization", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("WriteEtvFile", "W", "Write .etv file to disk", GH_ParamAccess.item, false);

            // Make all inputs optional to avoid yellow warnings
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Operation status and messages", GH_ParamAccess.item);
            pManager.AddGenericParameter("EttvModel", "M", "EttvModel object (not implemented)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Placeholder implementation
            var surfaces = new List<EttvSurface>();
            bool reorder = false;
            bool writeFile = false;

            // Get inputs (optional)
            DA.GetDataList(0, surfaces);
            DA.GetData(1, ref reorder);
            DA.GetData(2, ref writeFile);

            // TODO: Implement model creation and file writing
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "EttvModel: Implementation pending");
            DA.SetData(0, "Model creation not implemented");
            DA.SetData(1, null); // EttvModel placeholder
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.EttvModel.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("BBA8E894-9193-4563-8A4F-E4D197778691");
    }
}