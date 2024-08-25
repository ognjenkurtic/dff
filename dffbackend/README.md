Applying SHA-256 on the following string: 1234567887654321xxx1232023-01-012023-01-31550000
Applying SHA-256 on the following string: 12345678876543212023-01-012023-01-31550000
Applying SHA-256 on the following string: 1234567887654321xxx1232023-01-012023-01-31
Applying SHA-256 on the following string: 12322233312342

# Document instructions

# Backend Setup

1. Update `aplication.json`:

   Set the "Urls" field to your localhost, e.g., "http://localhost:5000".

2. Configure `secrets.json`:

   - Use the Manage User Secret extension in VSCode.

   - Set "Mysql" connection string:
     "ConnectionStrings": {
     "Mysql": "Server=localhost;Database=dff;User=root;Password=your_password;"
     }`

   Replace your_password with your MySQL password.

   - Set `AdminApiKey`:

   Generate a GUID online and set it in secrets.json:

   "AdminApiKey": "your_GUID_key"

3. Run the Backend:

   Execute `dotnet` run in the terminal to start the backend application.

# Frontend Setup

1. Update `auth.js`:

   Set apiUrl to the same localhost URL from the first step:

   `const apiUrl = "http://localhost:5000";`

   Ensure the URL does not have a trailing slash /.

2. Run the Frontend:

   Use Live Server in VSCode to run index.html.

# To access Swagger for API documentation

1. Open Swagger UI:

   `http://localhost:5000/api/index.html`

   Replace http://localhost:5000 with the localhost URL you set earlier if it's different.
