using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
namespace MovieXReview.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        public required string TagName { get; set; }

        public required string TagColor { get; set; }


    }

}
