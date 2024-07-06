namespace MDS.Model.Entity
{
    public class Comment
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }

        public string Content { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }

        public DateTime Date { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsQuestion { get; set; } = false;

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
