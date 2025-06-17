# ClientContactAPI

## Overview
ClientContactAPI is a .NET Web API for managing clients, templates, and communication within an organization. It supports JWT-based authentication, integrates Swagger for API testing and documentation, and includes xUnit tests to ensure application reliability.

---

## Configuration

Before running the application, update the connection string in the `appsettings.json` file.

### Database Connection

Set the connection string to point to your **test database**:

```json
"ConnectionStrings": {
  "connString": "your_connection_string_here"
}
```

### Database Initialization 

To create the required database tables:

1. Open the solution in Visual Studio

2. Ensure the database is accessible

3. Run the application (Start or Debug mode)

4. Access the endpoint **localhost:{port}/api/baseapi/createdb**, the application will automatically create the necessary tables if configured correctly.


### Swagger Testing

You can test the API through the built-in Swagger UI. To authenticate:

1. Use the Login method on Swagger with the following default credentials:

```json
{
  "username": "ADMIN",
  "hashPassword": "MDA4YZCWMZKYZTNHYMZIZDBMYTQ3YMJJMMVKOTZHYTK5YMQ0OWUXNTK3MJDMY2JHMGYYZTZHYMVIM2E5ZDYWMQ=="
}
```

2. Copy the returned JWT token

3. Click the Authorize button in Swagger UI and paste the token in the **value** field displayed in the dialog window.

