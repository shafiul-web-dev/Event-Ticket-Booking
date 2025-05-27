namespace Event_Ticket_Booking.DTO
{
	public class BookingDto
	{
		public string UserName { get; set; }
		public string EventName { get; set; }
		public DateTime EventDate { get; set; }
		public string TicketTier { get; set; } // Standard, VIP, Premium
		public decimal Price { get; set; }
		public int TicketQuantity { get; set; }
	}
}
