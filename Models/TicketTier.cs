namespace Event_Ticket_Booking.Models
{
	public class TicketTier
	{
		public int Id { get; set; }
		public int EventId { get; set; }
		public Event Event { get; set; }
		public string TierName { get; set; } // Standard, VIP, Premium
		public decimal Price { get; set; }
		public int AvailableTickets { get; set; }
		public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
	}
}
