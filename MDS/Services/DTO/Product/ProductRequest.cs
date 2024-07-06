﻿namespace MDS.Services.DTO.Product
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }
        public List<string> PictureUrls { get; set; }
        public string Description { get; set; }

        public bool Prescription { get; set; }

        public string ActiveIngredient { get; set; }

        public string DosageForm { get; set; }

        public string? Use { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
    }
}
