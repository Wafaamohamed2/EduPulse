using EduPulse.Models.Users;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EduPulse.Models
{
    public class Attendence
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }

        public string? FingerId { get; set; }

        private readonly SW_Entity _context;

        public Attendence(SW_Entity context)
        {
            _context = context;
        } 


        // Method to record attendance for a student
        #region  
        public void RecordAttendance(string fingerId)
        {
            // check if student has fingerid record , if the fingerId is invalid
            if (string.IsNullOrEmpty(fingerId))
            {
                Console.WriteLine("Invalid finger ID. Attendance not recorded.");
                return;
            }
            var student = _context.Students.FirstOrDefault(s => s.FingerId == fingerId);

          
            if (student == null)
            {
                Console.WriteLine("Student not found");
                return;
            }

            var record = new AttendenceRecord
            {
                StudentId = student.Id,
                AttendenceDate = DateTime.Now
            };

            _context.AttendenceRecords.Add(record);
            _context.SaveChanges();
            
            Console.WriteLine($"Attendance recorded for student {student.Name}");
        }
        #endregion



        // Method to check if a student has attended
        #region
        public bool CheckAttendance(string fingerId)
        {
            var student = _context.Students.FirstOrDefault(s => s.FingerId == fingerId);
            if (student == null)
            {
                return false;
            }
            return true;
        }
        #endregion


        // Method to calculate the attendance rate for a student
        #region
        public double Absence_Rate(string fingerId, DateTime startDate, DateTime endDate)
        {

            var student = _context.Students.FirstOrDefault(s => s.FingerId == fingerId);
            if (student == null)
            {
                return 100.0;
            }

            // num of days should student attend
            int totalDays = (int)(endDate - startDate).TotalDays + 1;

            // num of days student attended already
            int presentDays = _context.AttendenceRecords.Count(r => r.StudentId == student.Id &&
                                                        r.AttendenceDate.Date >= startDate.Date &&
                                                        r.AttendenceDate.Date <= endDate.Date &&
                                                        r.IsPresent == true);

            // calculate the attendance rate
            int absentDays = totalDays - presentDays;
            double absenceRate = (absentDays / (double)totalDays) * 100;

            return absenceRate;

        }

        #endregion



        //  relationships
        #region
        public int studentId { get; set; }  // foreign key

        public Student Student { get; set; } = new Student();


        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = new Teacher();

        #endregion
    }
}
