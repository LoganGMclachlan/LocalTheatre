using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Local_Theatre.Models
{
    // class for blog posts
    public class Post
    {
        // class attributes
        [Key]
        public int PostID { get; set; }

        [Required]
        [Display(Name = "Title:")]
        public string Title { get; set; }
        
        [ForeignKey("User")]
        public string Id { get; set; }// what user made post
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Category:")]
        public Categories Category { get; set; }

        [Display(Name = "Date Posted:")]
        public DateTime DatePosted { get; set; }
        
        public Boolean IsApproved { get; set; }// determines if post is visibke to other users
        
        public List<Comment> Comments { get; set; }// contains all comments made on post

        
    }

    public enum Categories
    {
        News,
        Announcment,
        Update,
        Review
    }
}