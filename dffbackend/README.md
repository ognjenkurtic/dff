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

   Set apiUrl to the same URL from the first step:

   `const apiUrl = "http://localhost:5000";`

   Ensure the URL does not have a trailing slash /.

2. Run the Frontend:

   Use Live Server in VSCode to run index.html.

# To access Swagger for API documentation

1. Open Swagger UI:

   `http://localhost:5000/api/index.html`

   Replace http://localhost:5000 with the localhost URL you set earlier if it's different.

# Publishing a Release (Ubuntu)

The backend is published as a **framework-dependent** build targeting **`linux-x64`**.
This means the Ubuntu server must have the **ASP.NET Core 6 runtime** installed
(the runtime is not bundled with the output).

0. Bump  version in the .csproj file

1. From the project root (`dffbackend/`), run:

   ```bash
   dotnet publish -c Release
   ```

2. The published files are written to:

   ```
   bin/Release/net6.0/publish/
   ```

3. Download a backup of the /var/www/faktorprovera.pks.rs folder from the server locally

4. Deploy the **entire contents** of `bin/Release/net6.0/publish/` to the Ubuntu server.

5. Deploy frontend files to the server. Make sure `auth.js` set for correct environment.

   `const apiUrl = "http://localhost:5000";` - LOCAL
   `const apiUrl = "https://dff.finspot.rs";` - UAT
   `const apiUrl = "https://faktorprovera.pks.rs";` - PROD


6. On the server, restart the backend app:

   ```
   systemctl restart kestrel-whisper.service
   ```

   Deployed `appsettings.json` should fetch the correct values from the systemd defined env vars.

7. git tag <version number>



