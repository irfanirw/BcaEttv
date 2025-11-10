using System;
using Grasshopper.Kernel;
using BcaEttvCore;

namespace BcaEttv
{
    public class ComputeEttvComponent : GH_Component
    {
        public ComputeEttvComponent()
          : base("ComputeEttv", "CE",
                 "Compute ETTV values for the model",
                 "BcaEttv", "Calculations")
        { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvModel", "M", "EttvModel object", GH_ParamAccess.item);
            pManager.AddBooleanParameter("RunComputation", "R", "Run ETTV computation", GH_ParamAccess.item, false);

            // Make all inputs optional to avoid yellow warnings
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EttvModel", "M", "Computed EttvModel object", GH_ParamAccess.item);
            pManager.AddNumberParameter("EttvValue", "V", "Computed ETTV value", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Pass", "P", "Whether ETTV passes compliance threshold", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            EttvModel model = null;
            bool runComputation = false;

            // Get inputs (optional)
            DA.GetData(0, ref model);
            DA.GetData(1, ref runComputation);

            if (model == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No EttvModel provided.");
                DA.SetData(0, null);
                DA.SetData(1, double.NaN);
                DA.SetData(2, false);
                return;
            }

            double? ettvValue = model.ComputationResult?.EttvValue;
            bool? pass = model.ComputationResult?.Pass;

            if (runComputation)
            {
                var calculator = new EttvCalculator(model);
                ettvValue = calculator.CalculateEttv();
                pass = model.ComputationResult?.Pass;
            }
            else if (ettvValue == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Set RunComputation to true to calculate ETTV.");
            }

            DA.SetData(0, model);
            DA.SetData(1, ettvValue ?? double.NaN);
            DA.SetData(2, pass ?? false);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("BcaEttv.Icons.ComputeEttv.png");
                return stream is null ? null : new System.Drawing.Bitmap(stream);
            }
        }

        public override Guid ComponentGuid => new Guid("C5F8A2D1-3E4B-4A5C-9D6E-7F8A9B0C1D2E");
    }
}
