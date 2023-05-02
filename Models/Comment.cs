using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Local_Theatre.Models
{
    // class for user comments
    public class Comment
    {
        // class attributes
        [Key]
        public int CommentID { get; set; }

        [ForeignKey("User")]
        public string Id { get; set; }// what user made post
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        [Display(Name = "Date Posted:")]
        public DateTime DatePosted { get; set; }
        
        public Boolean IsApproved { get; set; }// determines if comment is visibke to other users
        
        [ForeignKey("Post")]
        public int PostID { get; set; }// stores what post comment is on
        public Post Post { get; set; }
    }
}