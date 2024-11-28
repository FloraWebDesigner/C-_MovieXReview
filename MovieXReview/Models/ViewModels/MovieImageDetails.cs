namespace MovieXReview.Models.ViewModels
{
    public class MovieImageDetails
    {
        //An Image Page must have an image
        //FindImage(imageID)
        public required ImagesDto Image { get; set; }

        // An image may have a project associated to it
        // ListImagesForMovie(movieId)
        public MovieDto? Movie { get; set; }

    }
}
