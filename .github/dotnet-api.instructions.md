---
applyTo: "**/src/api/*.cs"
---

## API requirements

1. When writing a web API in C#, follow RESTful principles and use appropriate HTTP verbs (GET, POST, PUT, DELETE) for each action.
2. Use meaningful and consistent naming conventions for API endpoints (e.g., /readers, /books/{id}).
3. Return appropriate HTTP status codes for each API response (e.g., 200 OK, 201 Created, 404 Not Found).
4. Include metadata in API responses where applicable (e.g., pagination information, resource links).
5. Document API endpoints, request/response formats, and any authentication/authorization requirements.
6. Do not serialise responses to test, return as models defined in /src/api/model