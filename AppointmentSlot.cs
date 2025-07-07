public class AppointmentSlot
{
    public string TimeRange { get; set; }
    public bool IsReserved { get; set; }

    public AppointmentSlot(string timeRange)
    {
        TimeRange = timeRange;
        IsReserved = false;
    }
}