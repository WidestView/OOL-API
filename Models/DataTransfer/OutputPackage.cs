namespace OOL_API.Models.DataTransfer
{
    public class OutputPackage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal BaseValue { get; set; }

        public decimal PricePerPhoto { get; set; }

        public int? ImageQuantity { get; set; }

        public int? QuantityMultiplier { get; set; }

        public int? MaxIterations { get; set; }

        public bool Available { get; set; }
    }

#nullable enable

    public class OutputPackageHandler
    {
        public OutputPackage OutputFor(Package package)
        {
            return new OutputPackage
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                BaseValue = package.BaseValue,
                PricePerPhoto = package.PricePerPhoto,
                ImageQuantity = package.ImageQuantity,
                QuantityMultiplier = package.QuantityMultiplier,
                MaxIterations = package.MaxIterations,
                Available = package.Available
            };
        }
    }
}
