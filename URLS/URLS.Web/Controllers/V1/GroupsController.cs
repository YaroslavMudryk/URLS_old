using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.Group.GroupRole;
using URLS.Application.ViewModels.Post;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Application.ViewModels.Reaction;
using URLS.Application.ViewModels.User;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class GroupsController : ApiBaseController
{
    private readonly IGroupService _groupService;
    private readonly IPostService _postService;
    private readonly IGroupRoleService _groupRoleService;
    private readonly IGroupInviteService _groupInviteService;
    private readonly ICommentService _commentService;
    private readonly IGroupMemberService _groupMemberService;
    private readonly ISubjectService _subjectService;
    private readonly IIdentityService _identityService;
    private readonly IReactionService _reactionService;
    private readonly IExportService _exportService;
    public GroupsController(IGroupService groupService, IIdentityService identityService, ISubjectService subjectService, IGroupRoleService groupRoleService, IGroupMemberService groupMemberService, IGroupInviteService groupInviteService, IPostService postService, ICommentService commentService, IReactionService reactionService, IExportService exportService)
    {
        _groupService = groupService;
        _identityService = identityService;
        _subjectService = subjectService;
        _groupRoleService = groupRoleService;
        _groupMemberService = groupMemberService;
        _groupInviteService = groupInviteService;
        _postService = postService;
        _commentService = commentService;
        _reactionService = reactionService;
        _exportService = exportService;
    }

    #region Groups

    [HttpGet]
    public async Task<IActionResult> GetAllGroups(int offset = 0, int limit = 20)
    {
        return JsonResult(await _groupService.GetAllGroupsAsync(offset, limit));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(int id)
    {
        return JsonResult(await _groupService.GetGroupByIdAsync(id));
    }

    [HttpGet("{groupId}/export")]
    [AllowAnonymous]
    public async Task<IActionResult> ExportGroupMembers(int groupId)
    {
        return JsonResult(await _exportService.ExportGroupAsync(groupId));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups()
    {
        return JsonResult(await _groupService.GetUserGroupsAsync(_identityService.GetUserId()));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchGroups([MinLength(2)] string name, int? course = null, int? specialtyId = null, DateTime? from = null, DateTime? to = null, int offset = 0, int count = 20)
    {
        return JsonResult(await _groupService.SearchGroupsAsync(new SearchGroupOptions
        {
            Name = name,
            Course = course,
            SpecialtyId = specialtyId,
            From = from,
            To = to,
            Count = count,
            Offset = offset
        }));
    }

    [HttpPut("{groupId}/class-teacher")]
    public async Task<IActionResult> ModifyClassTeacherOfGroup(int groupId, [FromBody] GroupClassTeacherEditModel model)
    {
        model.GroupId = groupId;
        return JsonResult(await _groupService.UpdateClassTeacherGroupAsync(model));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] GroupCreateModel model)
    {
        return JsonResult(await _groupService.CreateGroupAsync(model));
    }

    [HttpPatch("{groupId}/increase-course")]
    public async Task<IActionResult> IncreaseCourseInGroup(int groupId)
    {
        return JsonResult(await _groupService.IncreaseCourseOfGroupAsync(groupId));
    }

    #endregion

    #region Members

    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetGroupMembers(int groupId, int offset = 0, int count = 20, int status = 0)
    {
        return JsonResult(await _groupMemberService.GetGroupMembersAsync(groupId, offset, count, status));
    }

    [HttpPost("{groupId}/members/accept-all")]
    public async Task<IActionResult> AcceptAllNewMembers(int groupId)
    {
        return JsonResult(await _groupMemberService.AcceptAllNewGroupMembersAsync(groupId));
    }

    [HttpGet("{groupId}/members/{memberId}")]
    public async Task<IActionResult> GetGroupMember(int groupId, int memberId)
    {
        return JsonResult(await _groupMemberService.GetGroupMemberByIdAsync(groupId, memberId));
    }

    [HttpPost("{groupId}/members/{memberId}/accept")]
    public async Task<IActionResult> AcceptNewMember(int groupId, int memberId, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UserEditModel editModel)
    {
        return JsonResult(await _groupMemberService.AcceptNewGroupMemberAsync(groupId, memberId, editModel));
    }

    [HttpDelete("{groupId}/members/{memberId}/reject")]
    public async Task<IActionResult> RejectNewMember(int groupId, int memberId)
    {
        return JsonResult(await _groupMemberService.RejectNewGroupMemberAsync(groupId, memberId));
    }

    [HttpPut("{groupId}/members/{memberId}")]
    public async Task<IActionResult> UpdateGroupMember(int groupId, int memberId, [FromBody] GroupMemberEditModel model)
    {
        model.GroupId = groupId;
        model.Id = memberId;
        return JsonResult(await _groupMemberService.UpdateGroupMemberAsync(model));
    }

    #endregion

    #region GroupRoles

    [HttpGet("roles")]
    public async Task<IActionResult> GetGroupRoles()
    {
        return JsonResult(await _groupRoleService.GetAllGroupRolesAsync());
    }

    [HttpGet("roles/{id}")]
    public async Task<IActionResult> GetUserGroupRoleById(int id)
    {
        return JsonResult(await _groupRoleService.GetGroupRoleByIdAsync(id));
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateUserGroupRole([FromBody] UserGroupRoleCreateModel model)
    {
        return JsonResult(await _groupRoleService.CreateGroupRoleAsync(model));
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateUserGroupRole(int id, [FromBody] UserGroupRoleEditModel model)
    {
        model.Id = id;
        return JsonResult(await _groupRoleService.UpdateGroupRoleAsync(model));
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> RemoveUserGroupRole(int id)
    {
        return JsonResult(await _groupRoleService.RemoveGroupRoleAsync(id));
    }

    #endregion

    #region Invites

    [HttpGet("{groupId}/invites")]
    public async Task<IActionResult> GetGroupInvites(int groupId)
    {
        return JsonResult(await _groupInviteService.GetGroupInvitesByGroupIdAsync(groupId));
    }

    [HttpPost("{groupId}/invites")]
    public async Task<IActionResult> CreateGroupInvite(int groupId, [FromBody] GroupInviteCreateModel model)
    {
        model.GroupId = groupId;
        return JsonResult(await _groupInviteService.CreateGroupInviteAsync(model));
    }

    [HttpPut("{groupId}/invites")]
    public async Task<IActionResult> UpdateGroupInvite(int groupId, [FromBody] GroupInviteEditModel model)
    {
        return JsonResult(await _groupInviteService.UpdateGroupInviteAsync(model));
    }

    [HttpDelete("{groupId}/invites/{id}")]
    public async Task<IActionResult> RemoveGroupInvite(int groupId, Guid id)
    {
        return JsonResult(await _groupInviteService.RemoveGroupInviteAsync(groupId, id));
    }

    #endregion

    #region Posts

    [HttpGet("{groupId}/posts")]
    public async Task<IActionResult> GetGroupPosts(int groupId, int offset = 0, int count = 20)
    {
        return JsonResult(await _postService.GetPostsByGroupIdAsync(groupId, offset, count));
    }

    [HttpPost("{groupId}/posts")]
    public async Task<IActionResult> CreatePost(int groupId, [FromBody] PostCreateModel model)
    {
        model.GroupId = groupId;
        return JsonResult(await _postService.CreatePostAsync(model));
    }

    [HttpPut("{groupId}/posts/{postId}")]
    public async Task<IActionResult> UpdatePost(int groupId, int postId, [FromBody] PostEditModel model)
    {
        model.Id = postId;
        model.GroupId = groupId;
        return JsonResult(await _postService.UpdatePostAsync(model));
    }

    [HttpGet("{groupId}/posts/{postId}")]
    public async Task<IActionResult> GetGroupPostById(int groupId, int postId)
    {
        return JsonResult(await _postService.GetPostByIdAsync(postId, groupId));
    }

    [HttpDelete("{groupId}/posts/{postId}")]
    public async Task<IActionResult> DeletePostById(int groupId, int postId)
    {
        return JsonResult(await _postService.RemovePostAsync(postId, groupId));
    }

    #endregion

    #region Comments

    [HttpGet("{groupId}/posts/{postId}/comments")]
    public async Task<IActionResult> GetPostComments(int groupId, int postId, int offset = 0, int count = 20)
    {
        return JsonResult(await _commentService.GetCommentsByPostIdAsync(groupId, postId, offset, count));
    }

    [HttpPost("{groupId}/posts/{postId}/comments")]
    public async Task<IActionResult> CreateComment(int groupId, int postId, [FromBody] CommentCreateModel model)
    {
        model.PostId = postId;
        model.GroupId = groupId;
        return JsonResult(await _commentService.CreateCommentAsync(model));
    }

    [HttpPut("{groupId}/posts/{postId}/comments/{commentId}")]
    public async Task<IActionResult> UpdateComment(int groupId, int postId, long commentId, [FromBody] CommentEditModel model)
    {
        model.PostId = postId;
        model.GroupId = groupId;
        return JsonResult(await _commentService.UpdateCommentAsync(model));
    }

    [HttpDelete("{groupId}/posts/{postId}/comments/{commentId}")]
    public async Task<IActionResult> RemoveComment(int groupId, int postId, long commentId)
    {
        return JsonResult(await _commentService.RemoveCommentAsync(groupId, postId, commentId));
    }

    #endregion

    #region Reactions

    [HttpPost("{groupId}/posts/{postId}/reactions")]
    public async Task<IActionResult> CreateReaction(int groupId, int postId, [FromBody] ReactionCreateModel model)
    {
        model.PostId = postId;
        return JsonResult(await _reactionService.CreateAsync(model));
    }

    [HttpDelete("{groupId}/posts/{postId}/reactions")]
    public async Task<IActionResult> DeleteReaction(int groupId, int postId)
    {
        var reactionToDelete = new ReactionCreateModel
        {
            PostId = postId
        };
        return JsonResult(await _reactionService.DeleteAsync(reactionToDelete));
    }

    [HttpGet("{groupId}/posts/{postId}/reactions")]
    public async Task<IActionResult> GetAllReactions(int groupId, int postId, int offset, int count)
    {
        return JsonResult(await _reactionService.GetAllByPostIdAsync(postId, offset, count));
    }

    [HttpGet("posts/reactions")]
    [AllowAnonymous]
    public IActionResult GetAllReactions() => JsonResult(_reactionService.GetAllReactions());

    #endregion

    #region Subjects

    [HttpGet("{groupId}/subjects")]
    public async Task<IActionResult> GetGroupSubjects(int groupId, int offset = 0, int count = 20, bool isCurrentSemester = false)
    {
        return JsonResult(await _subjectService.SearchSubjectsAsync(new SearchSubjectOptions
        {
            GroupId = groupId,
            Offset = offset,
            Count = count,
            IsCurrentSemestr = false
        }));
    }

    [HttpGet("{groupId}/subjects/{subjectId}")]
    public async Task<IActionResult> GetGroupSubject(int groupId, int subjectId)
    {
        return JsonResult(await _subjectService.GetGroupSubjectAsync(groupId, subjectId));
    }

    #endregion
}
