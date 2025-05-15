namespace EduPulse.Models
{
    public class UserDevice
    {
       
            public int Id { get; set; }
            public int UserId { get; set; }
            public string DeviceToken { get; set; } = string.Empty; 
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public bool IsActive { get; set; } = true;

    }
      
}
