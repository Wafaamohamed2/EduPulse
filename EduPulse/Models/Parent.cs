﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SW_Project.Models
{
    public class Parent : IUser
    {
       
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Confirm_Password { get; set; } = string.Empty;
        public bool Login(string email, string password)
        {
            if (this.Email == email && this.Password == password)
            {
                Console.WriteLine(" login successful!");
                return true;
            }
            Console.WriteLine(" login failed!");
            return false;
        }

        public bool SignUp(string name, string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                this.Name = name;
                this.Email = email;
                this.Password = password;
                Console.WriteLine(" signup successful!");
                return true;
            }
            Console.WriteLine(" signup failed!");
            return false;
        }


       
        public void ViewSonInf()
        {
            if (this.student != null) { 
                this.student.ViewStdInf();
            }
            else
            {
                Console.WriteLine("No student information available.");
            }
        }
        

        public string NotifyAboutGrades(string message)
        {
            return $"Notification sent to {Name} about student's grade: {message}";
        }


      
        // one to one relationship with student
        public int studentId { get; set; }  // foreign key
        public Student student { get; set;} = new Student();
     
    }
}
