﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAnime.Models.Entities.Identity
{
    public class Users : IdentityUser<int, UserLogins, UserRoles, UserClaims>
    {
        [MaxLength(200)]
        public string AvatarUrl { get; set; }
        public DateTime? BirthDay { get; set; }
        [MaxLength(250)]
        public string FullName { get; set; }
        [DataType(DataType.DateTime)] public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        [DataType(DataType.DateTime)] public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        [DataType(DataType.DateTime)] public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Ratings> Ratings { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<BlogComments> BlogComments { get; set; }
        public virtual ICollection<Favorites> Favorites { get; set; }

    }

    public class Roles : IdentityRole<int, UserRoles>
    {
    }

    public class UserLogins : IdentityUserLogin<int>
    {
        [ForeignKey(nameof(User))]
        public override int UserId { get; set; }
        public override string LoginProvider { get; set; }
        public override string ProviderKey { get; set; }

        public virtual Users User { get; set; }
    }

    public class UserRoles : IdentityUserRole<int>
    {
        [ForeignKey(nameof(Users))]
        public override int UserId { get; set; }
        public virtual Users Users { get; set; }


        [ForeignKey(nameof(Roles))]
        public override int RoleId { get; set; }
        public virtual Roles Roles { get; set; }
    }

    public class UserClaims : IdentityUserClaim<int>
    {
        [ForeignKey(nameof(User))]
        public override int UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
