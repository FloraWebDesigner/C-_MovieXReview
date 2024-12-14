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
8. Search funtions for Movie and Viewer

Next step: 
1. Link the logged-in user to the corresponding viewer to display their specific tickets and reviews. We explored user identity by adding customer service, an interface, and controllers. However, directly replacing Viewer with Customer is not feasible and  I plan to extend IdentityUser to incorporate Viewer data and functionalities.
2. Consider extra features: image plugin, pagination...

Team tasks:
1. Flora: movie, ticket and viewer entities
2. Suzanna: image, review and tag entities
3. Flora and Suzanna:  combination, navigation, authentication, search and user exploration. 

##https://localhost:7256/swagger/index.html
