using System.Collections.Generic;

namespace BcaEttvCore
{
    public class EttvConstruction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<EttvMaterial> EttvMaterials { get; set; }
        public double Uvalue { get; set; }

        public EttvConstruction()
        {
            Id = string.Empty;
            Name = string.Empty;
            EttvMaterials = new List<EttvMaterial>();
            Uvalue = 0.0;
        }

        public void CalculateUvalue(List<EttvMaterial> materials)
        {
            EttvMaterials = materials ?? new List<EttvMaterial>();
            Uvalue = UvalueCalculator.ComputeUValue(EttvMaterials);
        }
    }
}
