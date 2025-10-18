using TeacherGuide.Application.Interfaces;

namespace TeacherGuide.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
