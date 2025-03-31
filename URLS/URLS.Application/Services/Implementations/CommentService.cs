using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class CommentService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    IPermissionCommentService permissionCommentService,
    ICommonService commonService) : ICommentService
{
    public async Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
    {
        if (!await commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
            return Result<CommentViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

        if (!await commonService.IsExistAsync<Post>(s => s.Id == model.PostId))
            return Result<CommentViewModel>.NotFound(typeof(Post).NotFoundMessage(model.PostId));

        if (!await permissionCommentService.CanCreateCommentAsync(model.GroupId))
            return Result<CommentViewModel>.Forbiden();

        var newComment = new Comment
        {
            IsPublic = model.IsPublic,
            PostId = model.PostId,
            UserId = identityService.GetUserId(),
            Text = model.Text
        };
        newComment.PrepareToCreate(identityService);
        await db.Comments.AddAsync(newComment);
        await db.SaveChangesAsync();
        return Result<CommentViewModel>.Created(mapper.Map<CommentViewModel>(newComment));
    }

    public async Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20)
    {
        if (!await db.Posts.AnyAsync(s => s.Id == postId && s.GroupId == groupId))
            return Result<List<CommentViewModel>>.NotFound(typeof(Post).NotFoundMessage(postId));

        var query = db.Comments.AsNoTracking();

        if (!await permissionCommentService.CanViewAllCommentsAsync(groupId, postId))
            query = query.Where(s => s.IsPublic);

        var comments = await query
            .Where(x => x.PostId == postId)
            .OrderByDescending(s => s.CreatedAt)
            .Include(x => x.User)
            .Skip(skip).Take(count)
            .ToListAsync();

        var totalCount = await db.Comments.CountAsync(x => x.PostId == postId);

        return Result<List<CommentViewModel>>.SuccessList(mapper.Map<List<CommentViewModel>>(comments), Meta.FromMeta(totalCount, skip, count));
    }

    public async Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId)
    {
        if (!await db.Posts.AsNoTracking().AnyAsync(s => s.Id == postId && s.GroupId == groupId))
            return Result<bool>.NotFound("Post from this group not found");

        var commentToRemove = await db.Comments.FirstOrDefaultAsync(s => s.Id == commentId);
        if (commentToRemove == null)
            return Result<bool>.NotFound(typeof(Comment).NotFoundMessage(commentId));

        if (commentToRemove.PostId != postId)
            return Result<bool>.Forbiden();

        if (!identityService.IsAdministrator())
            if (commentToRemove.UserId != identityService.GetUserId())
                return Result<bool>.Forbiden();

        db.Comments.Remove(commentToRemove);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
    {
        var commentToUpdate = await db.Comments.FindAsync(model.Id);
        if (commentToUpdate == null)
            return Result<CommentViewModel>.NotFound(typeof(Comment).NotFoundMessage(model.Id));

        if (!identityService.IsAdministrator())
            if (commentToUpdate.UserId != identityService.GetUserId())
                return Result<CommentViewModel>.Forbiden();

        commentToUpdate.Text = model.Text;
        commentToUpdate.IsPublic = model.IsPublic;
        commentToUpdate.PrepareToUpdate(identityService);
        db.Comments.Update(commentToUpdate);
        await db.SaveChangesAsync();
        return Result<CommentViewModel>.SuccessWithData(mapper.Map<CommentViewModel>(commentToUpdate));
    }
}
