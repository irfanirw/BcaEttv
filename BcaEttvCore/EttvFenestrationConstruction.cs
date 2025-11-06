namespace BcaEttvCore
{
    public class EttvFenestrationConstruction : EttvConstruction
    {
        public double ScTotal { get; set; }
        public double Sc1 { get; set; }
        public double Sc2 { get; set; }

        public void CalculateScTotal(double sc1, double sc2)
        {
            Sc1 = sc1;
            Sc2 = sc2;
            ScTotal = sc1 * sc2;
        }
    }
}
