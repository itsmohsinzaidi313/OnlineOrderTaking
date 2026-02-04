namespace ExportService.Entities
{
    public class Flavour
    {
        public int FlavourId { get; set; }

        public string? FlavourName { get; set; }

        public int? CompanyId { get; set; }
        
        public bool IsActive { get; set; }
        public Flavour CopyWith(Flavour instance)
        {
            return new Flavour
            {
                FlavourId = instance.FlavourId,
                FlavourName = instance.FlavourName,
                CompanyId = instance.CompanyId,
                IsActive = instance.IsActive
            };
        }
    }
}
