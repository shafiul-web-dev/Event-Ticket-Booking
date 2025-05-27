namespace Event_Ticket_Booking.Models
{
	public class Booking
	{
		public int Id { get; set; }
		public int TicketTierId { get; set; }
		public TicketTier TicketTier { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int TicketQuantity { get; set; }
		public DateTime BookingDate { get; set; }
	}
}
