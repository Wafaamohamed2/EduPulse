# Student Management System
  # Project Description
     The Student Management System is a comprehensive platform designed to manage students, teachers, 
     and parents in a school or university setting. The system provides an easy-to-use interface for handling daily operations 
     such as user registration, exam management, push notifications ,tracking attendance , and integration with external services like Google Sheets and Google Forms..
     
## Features
 # 1- Attendance Management :
       - Record student attendance using a unique FingerId for biometric identification..
       - Calculate absence rates for students over a specified period and notify parents if 
         the absence rate exceeds 25%.
       - Support for fingerprint-based authentication to securely set and verify student 
         identities.  

 # 2- Exam Management :
      - Teachers can Create and manage exams with details such as name, time limit, and 
        Google Form links.
      - Students can view available exams, start exams, and access Google Form links.
      - Integration with Google Sheets to retrieve exam grades.
      - Track exam results with student-specific grades and submission dates.

 # 3- User Management :
      - Supports multiple user roles: Students, Teachers, and Parents, with secure login and 
        signup functionalities.
      - Profile management, including updates to name, email, phone number, and profile 
        picture.
      - Password management with secure hashing (BCrypt), password reset via email, and 
        change password functionality. 
      - Device registration for push notifications using Firebase Cloud Messaging (FCM).


  # 4- Notifications : 
      - Send push notifications to users for profile updates, exam assignments, and high 
        absence rates.
      - Email notifications for user registration, password resets, and profile updates.
      - Notify parents about their child's academic performance and attendance status.
 
  # 5- Database Integration :
      - Uses Entity Framework Core for efficient database operations.
      - Supports complex relationships: one-to-one, one-to-many, and many-to-many between 
        entities.
      - Configures relationships using Fluent API in the SW_Entity context.

  # 6- External Integrations :
      - Integration with Google Forms for exam creation and management via the 
        GoogleFormsExamAdapter.
      - Integration with Google Sheets to fetch exam grades and other data.
      - Firebase integration for push notifications to registered devices.   

  # 7- Security :
      - JWT-based authentication for secure API access.
      - Role-based authorization to restrict endpoints to specific user roles (Student, 
        Teacher, Parent).
      - Error handling middleware to manage exceptions and return consistent API responses.
      - Password reset tokens stored with expiry dates for secure password recovery.  

      
## Models
  # 1- Student :
      - Properties: Id, Name, Email, Password, Grade, Subject, Level, FingerId, 
        ProfilePictureUrl, PhoneNumber.
      - Relationships: Linked to Parent, Exams, and AttendenceRecords.  
      - Includes methods for login, signup, taking exams, set fingerprint, view profile, and 
        update profile.

  # 2- Teacher :
      - Properties: Id, Name, Email, Password, Subject, ProfilePictureUrl, PhoneNumber.
      - Relationships: Linked to Exams and AttendenceRecords.
      - Includes methods for login, signup, creating exams, and saving exams to the database.

  # 3- Parent :
      - Properties: Id, Name, Email, Password, Confirm_Password, StudentId, 
        ProfilePictureUrl, PhoneNumber.
      - Relationships: Linked to Student.
      - Includes methods for login, signup, viewing student information, and receiving 
        notifications about grades.

  # 4- AttendanceRecord :
      - Properties: Id, AttendenceDate, IsPresent, FingerId, StudentId, TeacherId.
      - Includes methods for recording attendance, checking attendance, and calculating 
         attendance rates.

  # 5- Exam :
      - Properties: Id, ExamName, TimeLimitInMinutes, GoogleFormLink, DeadlineDate, TeacherId.
      - Relationships: Linked to Student via ExamStudent and ExamResult.
      - Includes methods for Create exams, check expiration status (IsExamExpired), and 
        retrieve Google Form links.

  # 6- ExamStudent :
      - Represents the many-to-many relationship between Student and Exam.
      - Properties: StudentsId, ExamsId, Student, Exam.
      
  # 7- ExamResult :
      - Properties: Id, StudentId, ExamId, Grade, DateTaken.
      - Relationships: Linked to Student and Exam.
      - Purpose: Stores student-specific exam results.
      
  # 8- UserDevice :
      - Properties: Id, UserId, DeviceToken, IsActive.
      - Purpose: Stores device tokens for push notifications.     
  
  # 9- PasswordResetTokens :
      - Properties: Id, UserId, Token, ExpiryDate.
      - Purpose: Manages tokens for secure password reset functionality. 
   
## Controllers :
 # 1- AuthController :
      - Endpoints: Login, logout, register (Student, Teacher, Parent), forgot password, 
        change password.
      - Features: JWT token generation, password hashing/verification, and email 
        notifications for registration and password resets.  

  # 2- StudentController :
       - Endpoints: Record attendance, calculate absence rate, view available exams, set 
         fingerprint.
       - Features: Biometric attendance tracking, absence notifications, and exam access for 
         students.  

  # 3- TeacherController :
       - Endpoints: Create exams.
       - Features: Exam creation with Google Form integration for teachers.

  # 4- ExamController :
       - Endpoints: Create exams, view available exams, start exams.
       - Features: Exam management for teachers and students, with Google Form link handling.

  # 5- UserController :
       - Endpoints: Update user profile, update device token.
       - Features: Profile management and device registration for push notifications.

  # 6- UserDeviceController :
       - Endpoints: Register and unregister devices for push notifications.
       - Features: Manages device tokens for Firebase Cloud Messaging.


## Additional Components  

  # 1. Database Context (SW_Entity):
     - Manages database operations using Entity Framework Core.
     - Configures relationships (one-to-one, one-to-many, many-to-many) using Fluent API.
     
  # 2. NotificationService:
     - Handles email notifications for user registration, password resets, and profile 
       updates.
     - Sends push notifications for absence alerts and other events using Firebase.

  # 3. JwtTokenGenerator:
     - Singleton class for generating JWT tokens with user claims (ID, email, role).
     - Configured with issuer, audience, and key from application settings.
     
  # 4. GoogleFormsExamAdapter:
     - Adapts Google Forms for exam creation and link retrieval.
     - Placeholder implementation for creating and accessing Google Form-based exams. 

  # 5. GoogleSheetsService:
     - Integrates with Google Sheets API to fetch exam grades.
     - Uses OAuth2 for secure access to spreadsheets.

  # 6. ErrorHandlingMiddleware:
     - Catches and handles exceptions across the application.
     - Returns consistent JSON responses with error details.  

  # 7. DTOs (Data Transfer Objects):
     - Examples: UserDeviceDto, UpdateUserProfileDto, ExamDto, StudentRegisterDto, 
       ChangePasswordDto.
     - Purpose: Validate and transfer data between client and server.

## Technical Stack :
    - Framework: ASP.NET Core
    - Database: Entity Framework Core with SQL Server (or other compatible databases)
    - Authentication: JWT with BCrypt for password hashing
    - Notifications: Firebase Cloud Messaging for push notifications, SMTP for email 
     notifications
    - External APIs: Google Sheets API, Google Forms integration
    - Logging: Serilog for error logging
    - Security: Role-based authorization, secure password management, and token-based 
      password resets  
## Installation :
    1. Clone the repository: git clone <repository-url>
    2. Install dependencies: dotnet restore
    3. Configure the database connection string in appsettings.json.
    4. Set up Google API credentials (credentials.json) for Google Sheets integration.
    5. Configure Firebase for push notifications.
    6. Run migrations: dotnet ef migrations add InitialCreate and dotnet ef database update
    7. Start the application: dotnet run

## Future Enhancements :
    - Implement real-time exam grading and feedback.
    - Add support for multiple biometric authentication methods.
    - Enhance Google Forms integration with dynamic question management.
    - Introduce a dashboard for teachers and parents to monitor student progress.
     
