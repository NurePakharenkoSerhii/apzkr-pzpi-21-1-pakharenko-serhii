namespace API.Core.DTOs.Order;

public class OrderUpdateDto
{
    public int Frequency { get; set; }
    public DateTime StartTimeUTC { get; set; }
    public DateTime EndTimeUTC { get; set; }
}