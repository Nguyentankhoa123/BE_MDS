namespace MDS.Model.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string DetailCategory { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public List<string> PictureUrls { get; set; }

        public string Description { get; set; }

        public bool? Prescription { get; set; } // Thuốc có cần kê toa không

        public string? ActiveIngredient { get; set; } // Hoạt chất

        public string? DosageForm { get; set; } // Dạng bào chế

        public string Use { get; set; } // Công dụng

        public int Quantity { get; set; }
        public string Note { get; set; } // Lưu ý
        public string ProductType { get; set; }

        public Inventory Inventory { get; set; }

        public string DrugstoreId { get; set; }

        public ApplicationUser Drugstore { get; set; } // Nhà thuốc

        public DateTime? Created { get; set; } = DateTime.Now;

        public ICollection<Comment> Comments { get; set; }
    }
}
