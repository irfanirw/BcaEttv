using System.Collections.Generic;

namespace BcaEttvCore
{
    public class EttvModel
    {
        public List<EttvSurface> Surfaces { get; set; } = new();
        public double? EttvValue { get; set; }
        public bool? Pass { get; set; }
    }
}