using MDS.Services.Common;

namespace MDS.Services.DTO.Comment
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsQuestion { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class CommentObjectResponse : ObjectResponse<CommentResponse> { }

    public class CommentListObjectResponse : ObjectResponse<List<CommentResponse>> { }
}
