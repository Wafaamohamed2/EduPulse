namespace SW_Project.Models
{
    public class Exam
    {
        //Properties
        #region
        public int Id { get; set; }
        public string ExamName { get; set; }
        public int TimeLimitInMinutes { get; set; }
        public string? GoogleFormLink { get; set; }
        public List<string> Questions { get; set; } = new List<string>();
        public List<string> Answers { get; set; } = new List<string>();
        public int QuestionsCount => Questions.Count;
        #endregion

       public Exam() { }
        public Exam(string examname , int examtime) {
        
          ExamName = examname;
          TimeLimitInMinutes = examtime;
        }



        public void AddQuestion(string question , string answer)
        {
            Questions.Add(question);
            Answers.Add(answer);
        }
        public void RemoveQuestion(string question) {
            int index = Questions.IndexOf(question);
            if (index >0)
            {
                Questions.RemoveAt(index);
                Answers.RemoveAt(index);  //remove the answer with the question
            }

           
        }


        //many to many relationship with student
        public ICollection<Student> Students { get; set; } = new List<Student>();   
    }
}
