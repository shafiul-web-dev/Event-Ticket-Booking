namespace Event_Ticket_Booking.Models
{
	public class Event
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; } // Concert, Sports, Conference
		public DateTime EventDate { get; set; }
		public string Location { get; set; }
		public ICollection<TicketTier> TicketTiers { get; set; } = new List<TicketTier>();
	}
}
