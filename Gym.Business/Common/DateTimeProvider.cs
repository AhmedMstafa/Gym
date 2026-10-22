namespace Gym.Business.Common
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
        public DateTime DateTimeToday => DateTime.Today;
    }
}
