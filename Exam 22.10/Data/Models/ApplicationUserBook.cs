﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ApplicationUserBook
    {
        [Required]
        public string ApplicationUserId  { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser 	ApplicationUser  { get; set; }

        [Required]
        public int BookId  { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
    }
}