using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Comment;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommentObjectResponse> CreateAsync(CommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            int rightValue = 0;
            if (request.ParentId.HasValue)
            {
                var parentComment = await _context.Comments.FindAsync(request.ParentId.Value);
                if (parentComment == null)
                {
                    throw new NotFoundException("Parent comment not found");
                }

                rightValue = parentComment.Right;

                var commentsToUpdateRight = _context.Comments
                    .Where(c => c.ProductId == request.ProductId && c.Right >= rightValue);

                foreach (var commentToUpdate in commentsToUpdateRight)
                {
                    commentToUpdate.Right += 2;
                }

                var commentsToUpdateLeft = _context.Comments
                    .Where(c => c.ProductId == request.ProductId && c.Left > rightValue);

                foreach (var commentToUpdate in commentsToUpdateLeft)
                {
                    commentToUpdate.Left += 2;
                }

                await _context.SaveChangesAsync();
            }
            else
            {

                var maxRightValue = await _context.Comments
                    .Where(x => x.ProductId == request.ProductId)
                    .MaxAsync(x => (int?)x.Right);
                if (maxRightValue != null)
                {
                    rightValue = maxRightValue.Value + 1;
                }
                else
                {
                    rightValue = 1;
                }
            }
            comment.Left = rightValue;
            comment.Right = rightValue + 1;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var response = new CommentObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Comment created",
                Data = _mapper.Map<CommentResponse>(comment)
            };

            return response;
        }

        public async Task<CommentObjectResponse> DeleteAsync(int commentId, int productId)
        {
            var foundProduct = await _context.Products.FindAsync(productId);

            if (foundProduct == null)
            {
                throw new NotFoundException("Product not found");
            }

            var comment = await _context.Comments.FindAsync(commentId);

            var leftValue = comment.Left;
            var rightValue = comment.Right;

            var width = rightValue - leftValue + 1;

            var commentsToDelete = _context.Comments
                .Where(c => c.ProductId == productId && c.Left >= leftValue && c.Right <= rightValue);

            _context.Comments.RemoveRange(commentsToDelete);

            var commentsToUpdateRight = _context.Comments
                .Where(c => c.ProductId == productId && c.Right > rightValue);

            foreach (var commentToUpdate in commentsToUpdateRight)
            {
                commentToUpdate.Right -= width;
            }

            var commentsToUpdateLeft = _context.Comments
                .Where(c => c.ProductId == productId && c.Left > rightValue);

            foreach (var commentToUpdate in commentsToUpdateLeft)
            {
                commentToUpdate.Left -= width;
            }

            await _context.SaveChangesAsync();

            return new CommentObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Comments deleted"
            };
        }

        public async Task<CommentListObjectResponse> GetCommentsByParentIdAsync(int? parentId, int productId)
        {
            if (parentId.HasValue)
            {
                var parent = await _context.Comments.FindAsync(parentId.Value);
                if (parent == null)
                {
                    throw new NotFoundException("Not found comment for product");
                }

                var comments = await _context.Comments
                    .Where(c => c.ProductId == productId && c.Left > parent.Left && c.Right <= parent.Right)
                    .OrderBy(c => c.Left)
                    .ToListAsync();

                var response = new CommentListObjectResponse
                {
                    StatusCode = ResponseCode.OK,
                    Message = "Get comment",
                    Data = _mapper.Map<List<CommentResponse>>(comments)
                };

                return response;
            }
            else
            {
                var comments = await _context.Comments
                    .Where(c => c.ProductId == productId && c.ParentId == parentId)
                    .OrderBy(c => c.Left)
                    .ToListAsync();

                var response = new CommentListObjectResponse
                {
                    StatusCode = ResponseCode.OK,
                    Message = "Get comment",
                    Data = _mapper.Map<List<CommentResponse>>(comments)
                };

                return response;
            }
        }
    }
}
