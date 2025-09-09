# Job Board API

A comprehensive job board application built with .NET 9 and Clean Architecture principles. This system enables companies to post job vacancies, job seekers to apply for positions, and provides a complete recruitment management solution.

## 🚀 Features

### Core Functionality
- **Job Management**: Create, update, publish, and archive job vacancies
- **User Management**: Support for different user types (Job Seekers, Company Admins, Company Employees)
- **Application System**: Apply to jobs with resumes or file attachments
- **Company Profiles**: Manage company information, logos, and descriptions
- **Resume Management**: Create and manage professional resumes
- **Category System**: Organize jobs by categories
- **File Upload**: Secure file upload functionality for resumes and documents

### Technical Features
- **Clean Architecture**: Separated into Domain, Application, Infrastructure, and API layers
- **Authentication & Authorization**: JWT-based authentication with role-based access
- **Database**: PostgreSQL with Entity Framework Core
- **Logging**: Structured logging with Serilog and Seq
- **API Documentation**: OpenAPI/Swagger integration
- **Docker Support**: Full containerization with Docker Compose
- **Testing**: Unit tests for domain and application layers

## 🏗️ Architecture

The application follows Clean Architecture principles with the following layers:

```
src/
├── API/                    # Web API layer (Controllers, Endpoints, Middleware)
├── Application/            # Application layer (Commands, Queries, Services)
├── Domain/                 # Domain layer (Entities, Value Objects, Domain Services)
├── Infrastructure/         # Infrastructure layer (External services, Email, etc.)
└── Persistence/           # Data access layer (Entity Framework, Repositories)

tests/
├── Application.Tests/      # Application layer tests
└── Domain.Tests/          # Domain layer tests
```

### Domain Contexts
- **Identity Context**: User authentication and authorization
- **Job Posting Context**: Job vacancy management
- **Resume Posting Context**: Resume and profile management
- **Recruitment Context**: Application and hiring process management
- **Application Context**: General application settings and utilities

## 🛠️ Technology Stack

- **.NET 9**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM for data access
- **PostgreSQL**: Primary database
- **Docker & Docker Compose**: Containerization
- **Serilog**: Structured logging
- **Seq**: Log aggregation and analysis
- **Swagger/OpenAPI**: API documentation
- **JWT**: Authentication tokens
- **FluentValidation**: Input validation
- **BCrypt**: Password hashing
- **AWS S3**: File storage (configured)

## 🚦 Getting Started

### Prerequisites
- .NET 9 SDK
- Docker and Docker Compose
- PostgreSQL (if running locally without Docker)
- AWS S3 bucket (for file storage)

### Running with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd job-board
   ```

2. **Create environment file**
   ```bash
   cp .env.example .env
   ```
   Update the `.env` file with your configuration:
   ```env
   # Database Configuration
   POSTGRES_DB=jobboard
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=your_password
   POSTGRES_HOST=jobboard.database
   POSTGRES_PORT=5432
   
   # Logging Configuration
   SEQ_FIRSTRUN_ADMINUSERNAME=admin
   SEQ_FIRSTRUN_ADMINPASSWORDHASH=your_seq_password_hash
   
   # AWS S3 Configuration (Required for file uploads)
   AWS_ACCESS_KEY_ID=your_aws_access_key
   AWS_SECRET_ACCESS_KEY=your_aws_secret_key
   AWS_S3_BUCKET_NAME=your-s3-bucket-name
   AWS_S3_REGION=us-east-1
   ```

3. **Configure HTTPS certificates for Docker (if needed)**
   ```bash
   # Generate certificate for Docker development
   dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p your_password
   dotnet dev-certs https --trust
   ```

4. **Start the application**
   ```bash
   docker-compose up -d
   ```

5. **Access the application**
   - API: https://localhost:5001 or http://localhost:5000
   - API Documentation: https://localhost:5001 (Swagger UI)
   - Seq Logs: http://localhost:8081
   - Database: localhost:5432

### Running Locally

1. **Setup PostgreSQL database**
   - Install PostgreSQL
   - Create database named `jobboard`

2. **Configure HTTPS development certificates**
   ```bash
   # Generate and trust the HTTPS development certificate
   dotnet dev-certs https --clean
   dotnet dev-certs https --trust
   
   # For Docker development (if needed)
   dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p your_password
   dotnet dev-certs https --trust
   ```

3. **Update connection string**
   Update `appsettings.Development.json` with your database connection

4. **Run the application**
   ```bash
   cd src/API
   dotnet run
   ```

## 📚 API Documentation

Once the application is running, you can access the interactive API documentation at:
- Development: `https://localhost:5001`
- Production: Configure based on your deployment

### Main API Endpoints

#### Authentication
- `POST /users/register/job-seeker` - Register as job seeker
- `POST /users/register/company-admin` - Register as company admin
- `POST /users/login/email` - Login with email
- `POST /users/login/phone` - Login with phone number
- `POST /users/refresh-token` - Refresh authentication token

#### Vacancies
- `GET /vacancies` - Get all vacancies
- `POST /vacancies` - Create new vacancy
- `GET /vacancies/{id}` - Get vacancy by ID
- `PUT /vacancies/{id}/title` - Update vacancy title
- `PUT /vacancies/{id}/description` - Update vacancy description
- `PUT /vacancies/{id}/salary` - Update vacancy salary
- `POST /vacancies/{id}/publish` - Publish vacancy
- `POST /vacancies/{id}/archive` - Archive vacancy

#### Applications
- `POST /vacancies/{vacancyId}/apply-with-resume/{resumeId}` - Apply with existing resume
- `POST /vacancies/{vacancyId}/apply-with-file` - Apply with file upload

#### Companies
- `PUT /companies/name` - Update company name
- `PUT /companies/description` - Update company description
- `PUT /companies/logo` - Update company logo
- `PUT /companies/website` - Update company website

#### Resumes
- `GET /resumes` - Get all resumes
- `POST /resumes` - Create new resume
- `GET /resumes/{resumeId}` - Get resume by ID
- `GET /resumes/{userId}` - Get user's resumes
- `DELETE /resumes/{resumeId}` - Delete resume
- `PUT /resumes/{resumeId}/publish` - Publish resume
- `PUT /resumes/{resumeId}/draft` - Set resume as draft
- `PUT /resumes/{resumeId}/personal-info` - Update personal information
- `PUT /resumes/{resumeId}/contact-info` - Update contact information
- `PUT /resumes/{resumeId}/location` - Update location
- `PUT /resumes/{resumeId}/desired-position` - Update desired position
- `PUT /resumes/{resumeId}/salary` - Update salary expectations
- `PUT /resumes/{resumeId}/skills-description` - Update skills description

#### Resume Work Experience
- `POST /resumes/{resumeId}/work-experiences` - Add work experience
- `DELETE /resumes/{resumeId}/work-experiences/{workExperienceId}` - Remove work experience

#### Resume Education
- `POST /resumes/{resumeId}/educations` - Add education
- `DELETE /resumes/{resumeId}/educations/{educationId}` - Remove education

#### Resume Languages
- `POST /resumes/{resumeId}/languages` - Add language
- `DELETE /resumes/{resumeId}/languages/{languageId}` - Remove language

#### Resume Employment Types & Work Arrangements
- `GET /resumes/employment-types` - Get available employment types
- `GET /resumes/work-arrangements` - Get available work arrangements  
- `PUT /resumes/{resumeId}/employment-types/add` - Add employment type
- `PUT /resumes/{resumeId}/employment-types/remove` - Remove employment type
- `PUT /resumes/{resumeId}/work-arrangements/add` - Add work arrangement
- `PUT /resumes/{resumeId}/work-arrangements/remove` - Remove work arrangement

#### Files
- `GET /files/upload-url` - Get pre-signed URL for S3 file upload

## 🧪 Testing

Run the test suite:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Domain.Tests
dotnet test tests/Application.Tests
```

## 🔧 Troubleshooting

### HTTPS Certificate Issues

If you encounter HTTPS certificate errors during development:

**For Local Development:**
```bash
# Clean existing certificates
dotnet dev-certs https --clean

# Generate new certificate and trust it
dotnet dev-certs https --trust

# Verify certificate installation
dotnet dev-certs https --check --trust
```

**For Docker Development:**
```bash
# Generate certificate for Docker with password
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p YourSecurePassword

# Trust the certificate
dotnet dev-certs https --trust

# Update docker-compose.yml or .env with the certificate password
# ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword
```

**Common Issues:**
- **"Unable to configure HTTPS endpoint"**: Ensure the certificate is trusted and the password matches
- **Browser security warnings**: Clear browser cache and ensure the certificate is properly trusted
- **Docker certificate mounting**: Verify the certificate path in `docker-compose.yml` matches your system

## 🔧 Configuration

### Environment Variables

Key configuration options in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=jobboard;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "job-board-api",
    "Audience": "job-board-client",
    "ExpirationInMinutes": 60
  },
  "AWS": {
    "S3": {
      "BucketName": "your-s3-bucket",
      "Region": "us-east-1"
    }
  }
}
```

### AWS S3 Configuration

The application requires AWS S3 for file storage (resumes, company logos, etc.). You need to:

1. **Create an S3 bucket** in your AWS account
2. **Configure IAM permissions** for the bucket access
3. **Set environment variables** or update `appsettings.json`:

**Environment Variables:**
```env
AWS_ACCESS_KEY_ID=your_aws_access_key
AWS_SECRET_ACCESS_KEY=your_aws_secret_key
AWS_S3_BUCKET_NAME=your-s3-bucket-name
AWS_S3_REGION=us-east-1
```

**Required S3 Bucket Permissions:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:PutObject",
        "s3:DeleteObject"
      ],
      "Resource": "arn:aws:s3:::your-s3-bucket-name/*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "s3:ListBucket"
      ],
      "Resource": "arn:aws:s3:::your-s3-bucket-name"
    }
  ]
}
```

**Alternative: LocalStack for Development**
For local development, you can use LocalStack to simulate S3:
```bash
docker run --rm -p 4566:4566 localstack/localstack
```

## 📝 Development

### Project Structure
- **Clean Architecture**: Each layer has clear responsibilities
- **CQRS Pattern**: Commands and Queries are separated
- **Domain-Driven Design**: Rich domain models with business logic
- **Dependency Injection**: Uses built-in .NET DI container
- **Minimal APIs**: Uses .NET minimal API endpoints

### Code Quality
- **StyleCop**: Code style analysis
- **SonarAnalyzer**: Code quality analysis
- **Global Exception Handling**: Centralized error handling
- **Structured Logging**: Comprehensive logging with Serilog

## 🐳 Docker Services

The application uses the following Docker services:

- **API** (`jobboard-api`): Main application on ports 5000/5001
- **Database** (`jobboard-database`): PostgreSQL on port 5432
- **Seq** (`jobboard-seq`): Log aggregation on ports 5341/8081

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)