# Student Management System
  # Project Description
     The Student Management System is a comprehensive platform designed to manage students, teachers, 
     and parents in a school or university setting. The system provides an easy-to-use interface for handling daily operations 
     such as student registration, exam creation, sending notifications to parents, and tracking attendance.
     
# Features
  1- Attendance Management :
      - Record student attendance using a unique FingerId.
      - Calculate absence rates for students over a specified period.

  2- Exam Management :
      - Create and manage exams with questions and answers.
      - Allow students to take exams and preview their results.

  3- User Management :
      - Support for students, teachers, and parents with login and signup functionalities.
      - Maintain relationships between students, teachers, and parents.
 
  4- Database Integration :
      - Uses Entity Framework Core for database operations.
      - Supports relationships such as one-to-one, one-to-many, and many-to-many between entities.

 # Models
   1- Student :
      - Represents a student with properties like Id, Name, Email, Password, Grade, Subject, Level, and Absences.
      - Includes methods for login, signup, taking exams, and viewing student information.

   2- Teacher :
      - Represents a teacher with properties like Id, Name, Email, Password, and Subject.
      - Includes methods for login, signup, creating exams, and saving exams to the database.

   3- Parent :
      - Represents a parent with properties like Id, Name, Email, Password, and Confirm_Password.
      - Includes methods for login, signup, viewing student information, and receiving notifications about grades.

   4- Attendence :
      - Represents attendance records with properties like Id, Date, IsPresent, and FingerId.
      - Includes methods for recording attendance, checking attendance, and calculating attendance rates.

   5- Exam :
      - Represents an exam with properties like Id, ExamName, TimeLimitInMinutes, GoogleFormLink, Questions, and Answers.
      - Includes methods for adding and removing questions.

   6- StudentSubject :
      - Represents the relationship between students and teachers for subjects.

   7- AttendenceRecord :
      - Represents a record of attendance with properties like Id, AttendenceDate, IsPresent, FingerId, StudentId, and TeacherId.     


# Controllers :
  1- tudentController :
      - Provides API endpoints for recording attendance and calculating absence rates.

# Database Context
  ~ SW_Entity :
     - Manages the database context and relationships between entities using Entity Framework Core.
     - Configures relationships such as one-to-one, one-to-many, and many-to-many using Fluent API.

      
