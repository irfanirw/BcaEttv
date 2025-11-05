namespace BcaEttvCore
{
    public class EttvMaterial
    {
        public string Name { get; set; }
        public double ThermalConductivity { get; set; }
        public double Thickness { get; set; }

        public EttvMaterial()
        {
            Name = "DefaultEttvMat";
            ThermalConductivity = 1.0;
            Thickness = 10;
        }
    }
}