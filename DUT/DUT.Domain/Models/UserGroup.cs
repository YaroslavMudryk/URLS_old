﻿namespace DUT.Domain.Models
{
    public class UserGroup : BaseModel<int>
    {
        public bool IsAdmin { get; set; }
        public string Title { get; set; }
        public int UserGroupRoleId { get; set; }
        public UserGroupRole UserGroupRole { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}