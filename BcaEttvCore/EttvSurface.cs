using Rhino.Geometry;

namespace BcaEttvCore
{
    public class EttvSurface
    {
        private EttvConstruction _construction;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; private set; }
        public Mesh Geometry { get; set; }

        public EttvConstruction Construction
        {
            get => _construction;
            set
            {
                _construction = value;
                // Auto-set Type based on construction type
                if (value is EttvOpaqueConstruction)
                    Type = "Wall";
                else if (value is EttvFenestrationConstruction)
                    Type = "Fenestration";
                else
                    Type = "Unknown";
            }
        }

        public EttvSurface()
        {
            Name = string.Empty;
            Type = "Unknown";
        }
    }


}