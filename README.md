Setup

Install Docker and run:

docker-compose up --build

This will start containers for the API, frontend, and database.
	•	Frontend: http://localhost:3000
	•	Backend: http://localhost:5000

 API Endpoints
 Login
POST /api/users/login
Body: { "login": "string", "password": "string" }
Returns: { id, login, token, expiresIn }
	•	
 Register
POST /api/users/register
Body: { "login": "string", "password": "string" }
Returns: { id, login, passwordHash, role }
	•	
 Shorten URL
POST /shorten
Body: string (long URL)
Returns: string (short URL)
	•	
 Redirect
GET /{shortCode} → redirects to original URL
	•	
 Delete URL
DELETE /url/{id} → deletes a URL entry
	•	
 URL Details
GET /url/{id}/details → returns { id, longUrl, shortUrl, createdByUserId, timestamp }
