using QuizManagement.Data.Enum;

namespace QuizManagement.Data.Interfaces
{
    public interface ISwitchable
    {
        Status Status { get; set; }
    }
}