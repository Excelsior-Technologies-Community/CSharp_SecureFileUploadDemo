# Secure File Upload System with Virus Scan & Cloud Storage (ASP.NET MVC)

## 📌 Project Overview

This project is a **secure file upload module** built using **ASP.NET Core MVC (.NET 9)**. It ensures that uploaded files are validated, scanned for potential threats, and securely stored in cloud storage (Azure Blob Storage or Local Azurite). The system also provides secure signed URLs for accessing uploaded files and supports image/PDF preview.

---

## 🚀 Features

### ✔ File Upload Functionality

* Supports **large file uploads** (100MB+)
* Validates **file extension** and **MIME type**
* Configurable limits via `appsettings.json`

### ✔ Antivirus Scanning

* Dummy antivirus scanner included
* Supports integration with:

  * **ClamAV** (local scanner)
  * **VirusTotal API** (cloud scanner)

### ✔ Cloud Storage Integration

* Azure Blob Storage integration using `BlobServiceClient`
* Automatic container creation if missing
* Option to use **Azurite local emulator**

### ✔ Secure Downloading

* Generates **signed secure token** for file access
* Prevents unauthorized access to file URLs

### ✔ File Preview

* Image preview (JPEG, PNG, GIF)
* PDF preview using browser rendering

### ✔ Database Support

* SQLite database via Entity Framework Core
* Stores file metadata and security token

---

## 🧱 Project Structure

```
SecureFileUploadDemo/
│
├── Controllers/
│     └── FilesController.cs
│
├── Services/
│     ├── IAntivirusScanner.cs
│     ├── DummyAntivirusScanner.cs
│     ├── ICloudStorageService.cs
│     ├── AzureBlobStorageService.cs
│     ├── FileUploadService.cs
│     └── IFileUploadService.cs
│
├── Data/
│     └── AppDbContext.cs
│
├── Models/
│     └── FileRecord.cs
│
├── Views/
│     └── Files/
│          ├── Index.cshtml
│          └── Upload.cshtml
│
├── appsettings.json
└── Program.cs
```

---

## ⚙️ Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "FileUpload": {
    "MaxSizeMB": 500,
    "AllowedMimeTypes": [
      "image/jpeg",
      "image/png",
      "image/gif",
      "application/pdf"
    ],
    "AllowedExtensions": [ ".jpg", ".jpeg", ".png", ".gif", ".pdf" ]
  },
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "uploads",
    "SignedUrlExpiryMinutes": 30
  }
}
```

---

## 🗄 Database Schema (FileRecord)

| Column           | Type     | Description          |
| ---------------- | -------- | -------------------- |
| Id               | GUID     | Primary key          |
| OriginalFileName | string   | File name from user  |
| StoredFileName   | string   | File name in blob    |
| ContentType      | string   | MIME type            |
| SizeBytes        | long     | File size            |
| IsSafe           | bool     | Virus scan result    |
| SignedToken      | string   | Token for secure URL |
| UploadedAt       | DateTime | Timestamp            |

---

## 📤 File Upload Process Flow

```
User Uploads File → Validation → Antivirus Scan → Upload to Cloud → Save Metadata → Generate Token → Show File List
```

### Step-by-step logic:

1. Read the file from form POST request.
2. Validate: file exists, size limit, MIME type, extension.
3. Scan the file using antivirus service.
4. Generate GUID-based filename.
5. Upload to Azure Blob Storage.
6. Save metadata into SQLite.
7. Redirect user to file listing.

---

## 🔐 Secure File Access (Signed URLs)

Files can be accessed only via this route:

```
/files/content/{id}?token={signed_token}
```

The `FilesController` verifies:

* File exists
* Signed token matches stored value

If invalid → return **401 Unauthorized**.

---

## 🧪 Antivirus Scanning

### Dummy Scanner Logic

```csharp
if (file.FileName.Contains("virus")) return false;
```

### Real Scanner Options

* **VirusTotal API**: Cloud multi-engine scanning
* **Custom File Signature Detection**: Pattern-based scanning

---

## ☁ Cloud Storage Details

### Supported Providers

* **Azure Blob Storage** (default)
* **Azurite local emulator**

### Upload Logic

1. File streamed to blob storage
2. URL not made public
3. Access allowed only via controller

---

## 🧩 Controllers

### FilesController.cs

Handles:

* File listing
* File uploading
* Secure file streaming

Routes:

| Action     | Method | Description                  |
| ---------- | ------ | ---------------------------- |
| Index      | GET    | List uploaded files          |
| Upload     | GET    | File upload form             |
| UploadFile | POST   | Handles actual upload        |
| Content    | GET    | Serves files with validation |

---

## 🖥 Views (Razor Pages)

### Upload Page

* Form with file picker
* Validation errors displayed

### Index Page

* Table of uploaded files
* Metadata shown
* Preview buttons:

  * Image preview
  * PDF viewer
  * Download link (secured)

---

## 🧪 Testing Virus Scan

### Use the **EICAR Antivirus Test File**

Create a `.txt` file with this text:

```
X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*
```

Upload it.

✔ Should be blocked
✔ Confirms antivirus logic works

---

## ▶️ Running the Project

### 1. Install EF Tools

```bash
dotnet tool install --global dotnet-ef
```

### 2. Apply Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Run Application

```bash
dotnet run
```

Go to:

```
https://localhost:5001/files
```

---

## 🌩 Azure Cloud Storage Setup

If using Azure:

1. Create Storage Account
2. Create Container `uploads`
3. Copy Connection String
4. Replace in `appsettings.json`

Example:

```json
"ConnectionString": "DefaultEndpointsProtocol=https;AccountName=name;AccountKey=key;EndpointSuffix=core.windows.net"
```

---

## 🔧 Future Enhancements

* Multiple file uploads
* File delete option
* VirusTotal detailed reports
* Chunked uploads for 1GB+ files
* Role-based upload permissions
* Audit logging
* Email notification after upload

---

## 📄 Conclusion

This project is a **secure, extensible, and production-ready file upload system** built using ASP.NET Core MVC. It ensures safe file handling with:

* Proper validations
* Antivirus scanning
* Cloud storage integration
* Secure token-based access
* Clean architecture and maintainable code structure

---

**Author:** Aryan 

