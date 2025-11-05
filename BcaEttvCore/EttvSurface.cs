namespace BcaEttvCore
{
    // Keep Core free of Rhino types; this is just a placeholder DTO.
    public class EttvSurface
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public EttvConstruction? Construction { get; set; }
    }
}