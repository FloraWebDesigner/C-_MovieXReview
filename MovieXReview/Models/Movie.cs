﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieXReview.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        public string MovieName { get; set; }

        public int? Year { get; set; }
        public string? Introduction { get; set; }
        public float Rate { get; set; }
        public string? Duration { get; set; }
        public string? Director { get; set; }
        public string? Star { get; set; }
        public int? TicketQuantity { get; set; }

        //A movie has many reviews
        public ICollection<Review>? Reviews { get; set; }

        //A movie releases many tickets
        public ICollection<Ticket>? Tickets { get; set; }




    }
}
