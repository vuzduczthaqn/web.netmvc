using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAnime.Models.Entities
{
    public partial class Blogs
    {
        public Blogs()
        {
            BlogCategories = new HashSet<BlogCategories>();
            BlogComments = new HashSet<BlogComments>();
        }
        public int Id { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }

        [MaxLength(250)]
        public string Slug { get; set; }
        [MaxLength(250)]
        public string ImageUrl { get; set; }

        public string Content { get; set; }

        public virtual ICollection<BlogComments> BlogComments { get; set; }
        [DataType(DataType.DateTime)] public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        [DataType(DataType.DateTime)] public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        [DataType(DataType.DateTime)] public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual ICollection<BlogCategories> BlogCategories { get; set; }
    }
}
