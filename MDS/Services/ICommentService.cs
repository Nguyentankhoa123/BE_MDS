using MDS.Services.DTO.Comment;

namespace MDS.Services
{
    public interface ICommentService
    {
        Task<CommentObjectResponse> CreateAsync(CommentRequest request);
        Task<CommentListObjectResponse> GetCommentsByParentIdAsync(int? parentId, int productId);
        Task<CommentObjectResponse> DeleteAsync(int commentId, int productId);
    }
}
