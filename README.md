# C# Creative Collaboration Project – Movie & Review
Entity: Movie, Ticket, Viewer, Review, Image, Tag.

DRM:
Movie X Ticket - 1: M
Movie X Review - 1: M
Movie X Image - 1: M
Movie X Tag - M: M
Movie X Viewer  -M: M
Viewer X Review - 1: M

The project includes:
1. CRUD for the above 6 entities.
2. Ticket sold/avaialable for given movies
3. Multiple colorful tags for different movies in terms of movie type and style
4. Multiple images onto the same movie(image new and image edit for uploads)
5. Viewer rates onto different movie within the review section
6. Create ticket from movie/viewer
7. Authentication: admin and user: admin - CRUD movie, tags; user - CRUD reviews, tickets

Next step: 
1. Link login user to viewers to show the exact ticket/review for the viewer/user
2. Add more navigation buttons on each page to make the direction more smooth
3. Consider extra features: plugin, pagination...

Team tasks:
1. Flora: movie, ticket and viewer entities
2. Suzanna: image, review and tag entities
3. Flora and Suzanna:  combination, navigation, authentication and other features. 

##https://localhost:7256/swagger/index.html
