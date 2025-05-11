# document-management-app
# 📦 Project Title

A brief description of your project goes here.

---

## 📥 How to Clone This Repository

Follow the steps below to clone this repository to your local machine:

### 1. Install Git

Download and install Git from [https://git-scm.com](https://git-scm.com) if you don't already have it installed.

### 2. Open Terminal or Command Prompt

- **Windows**: Use Git Bash or Command Prompt  
- **macOS/Linux**: Use the built-in Terminal

### 3. Navigate to the Directory Where You Want to Clone the Repo

bash
cd path/to/your/folder
4. Copy the Repository URL
Go to this GitHub repository page

Click the green "Code" button

Copy the URL (choose HTTPS or SSH)

5. Run the Clone Command
Using HTTPS:

bash
Copy
Edit
git clone https://github.com/username/repository-name.git
Or using SSH:

bash
Copy
Edit
git clone git@github.com:username/repository-name.git
6. Navigate Into the Cloned Repository
bash
Copy
Edit
cd repository-name
7. (Optional) Verify the Remote URL
bash
Copy
Edit
git remote -v

📘 Document Management System – User Manual
A full-stack document management application designed with secure authentication, role-based access control, and local file system storage.

📌 Overview
The Document Management System (DMS) allows users to:

Register and log in securely via JWT authentication

Upload and download documents

View paginated lists of uploaded files

Leverage role-based access (Admin/User)

Store files in a local directory (no external cloud storage)

Interact with a PostgreSQL backend and Angular frontend

This system is designed with Clean Architecture, supporting modularity and testability. Ideal for small-to-medium organizations, internal tools, or prototype systems without external cloud dependency.

🧰 Technology Stack
Layer	Technology
Backend	ASP.NET Core (.NET 8)
ORM	Entity Framework Core (PostgreSQL)
Frontend	Angular
Storage	Local file system (e.g., /Storage)
Database	PostgreSQL
Testing	xUnit
(Optional)	Spring Boot (Java) – Document Ingestion

🧑‍💻 Prerequisites
Make sure the following tools are installed on your system:

General
.NET 8 SDK

Node.js (LTS)

Angular CLI:
Install via npm install -g @angular/cli

Database
PostgreSQL 13+

Optional (for ingestion service)
Java 17+

Maven (3.8+)

📁 Project Structure
perl
Copy
Edit
document-management-system/
├── DocumentManagement.Application/       # Application logic
├── DocumentManagement.Domain/            # Domain models
├── DocumentManagement.Infrastructure/    # Infrastructure services
├── DocumentManagement.WebAPI/            # ASP.NET Core Web API
├── DocumentManagement.Tests/             # Unit/Integration tests
├── frontend/                              # Angular frontend
├── ingestion/                             # Spring Boot ingestion (optional)
└── README.md
🔧 System Setup
1. Clone the Repository
bash
Copy
Edit
git clone https://github.com/your-username/document-management-system.git
cd document-management-system
2. Set Up PostgreSQL
Start your PostgreSQL server.

Create a new database named:

sql
Copy
Edit
CREATE DATABASE "DocumentDB";
(Optional) Create a PostgreSQL user and grant privileges if needed.

3. Configure Backend
Navigate to the backend directory:

bash
Copy
Edit
cd DocumentManagement.WebAPI
Open appsettings.json and configure it as follows:

json
Copy
Edit
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=DocumentDB;Username=postgres;Password=yourpassword"
  },
  "Storage": {
    "LocalFolderPath": "C:\\DocumentStorage"  // Use "./Storage" for relative path
  },
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "your-app",
    "Audience": "your-app"
  }
}
Apply EF migrations and start the API:

bash
Copy
Edit
dotnet restore
dotnet ef database update
dotnet run
Once the server is running, the API will be available at:
https://localhost:5001/api/

4. Configure Frontend (Angular)
Open a new terminal:

bash
Copy
Edit
cd ../frontend
npm install
ng serve
Visit the frontend at:
http://localhost:4200

5. (Optional) Start the Ingestion Module
If using the Spring Boot ingestion module:

bash
Copy
Edit
cd ../ingestion
./mvnw spring-boot:run
Make sure it connects to the same PostgreSQL instance or service bus if required.

🔐 Admin User Setup
By default, the application seeds an Admin user into the users table (only if it does not exist).

📝 To Verify:
Run this SQL query in PostgreSQL:

sql
Copy
Edit
SELECT username, role FROM users WHERE role = 'Admin';
If the admin user does not exist, you can:

Register manually via frontend

Use the backend to seed a known admin with hashed password

Insert directly into the database with pre-hashed password (if you know the hashing algorithm)

🔁 Optional Manual Insert Example
(Assumes you generate a hashed password using your app's hashing logic)

sql
Copy
Edit
INSERT INTO users (username, password_hash, role)
VALUES ('admin', '<hashed_password_here>', 'Admin');
🔗 Key API Endpoints
Base URL: https://localhost:5001/api/

Method	Endpoint	Description
GET	/documents	Retrieve all uploaded documents
GET	/documents/paged?page=1&size=10	Retrieve paginated documents list
POST	/documents/upload	Upload a new document
GET	/documents/{id}	Download a specific document

Authentication is required for most endpoints via JWT.

🧪 Running Backend Tests
To run automated tests for backend logic:

bash
Copy
Edit
cd ../DocumentManagement.Tests
dotnet test
🗂️ Local Storage Directory
Uploaded files are stored in the local file system.
Ensure the path specified in appsettings.json exists:

Example:

makefile
Copy
Edit
C:\DocumentStorage
Or, for relative paths:

pgsql
Copy
Edit
./Storage
Your application will write documents to this directory.

🧑 User Roles
Role	Description
Admin	Can view, upload, and manage all documents
User	Can upload and view only their own files

Role is assigned at user creation or via admin interface (if available).

🛡️ Security Notes
Passwords are hashed using a secure algorithm (e.g., BCrypt or SHA256).

JWT tokens are used for secure API authentication.

Users must log in to upload/download documents.

🚀 Deployment to Microsoft Azure
This section guides you through deploying the Document Management System to Azure using Azure App Services and Azure Database for PostgreSQL.

🔹 Step 1: Prepare Azure Resources
Create a PostgreSQL Flexible Server

Use the Azure Portal or CLI

Example settings:

DB name: DocumentDB

Admin user: postgres

Enable public access or configure VNET/firewall

Allow SSL or configure your app to use SslMode=Disable (for development)

Create Azure Storage Folder Alternative

Since the app uses local file storage, App Service will persist files only temporarily.

For long-term storage:

Convert to Azure Blob (recommended), or

Use D:\home\site\wwwroot\storage path for temporary file retention

🔹 Step 2: Deploy Backend (.NET Web API)
Publish with Visual Studio / CLI

Right-click DocumentManagement.WebAPI → Publish

Or use the CLI:

bash
Copy
Edit
dotnet publish -c Release
Deploy to Azure App Service

In Azure Portal:

Create a new App Service (Linux or Windows)

Choose runtime: .NET 8 (LTS)

Deploy via:

Visual Studio

GitHub Actions

Azure CLI

Zip Deploy

Update App Settings in Azure

Go to Configuration > Application settings in App Service, and set:

plaintext
Copy
Edit
ConnectionStrings__DefaultConnection = Host=your-azure-db.postgres.database.azure.com;Port=5432;Database=DocumentDB;Username=postgres;Password=YourPassword;SslMode=Require
Storage__LocalFolderPath = D:\home\site\wwwroot\storage
Jwt__Key = your_jwt_secret_key
Jwt__Issuer = your-app
Jwt__Audience = your-app
📌 Important: Ensure D:\home\site\wwwroot\storage exists or is created by the application.

🔹 Step 3: Deploy Angular Frontend
You can choose one of the following options:

Option A: Azure Static Web Apps (Recommended)
Push the Angular project (frontend/) to a GitHub repository.

In Azure, create a new Static Web App.

Connect it to the repo and set:

App location: frontend

Output location: dist/<project-name>

Option B: Azure App Service (Node)
Build the Angular app:

bash
Copy
Edit
cd frontend
ng build --configuration production
Deploy the dist/ folder to another Azure App Service (Node.js) or serve it from the backend using UseStaticFiles().

🔹 Step 4: Configure CORS
If frontend and backend are on separate domains, add CORS rules:

In Program.cs or Startup.cs of your API:

csharp
Copy
Edit
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        builder => builder.WithOrigins("https://your-angular-url")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

app.UseCors("AllowAngularClient");
🔹 Step 5: Secure Your App
Use HTTPS

Rotate JWT keys periodically

Lock down PostgreSQL access

Monitor file system storage if staying on local storage

✅ Deployment Complete!

You now have a live, cloud-hosted version of your Document Management System accessible from anywhere.
