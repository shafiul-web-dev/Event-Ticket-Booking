using Event_Ticket_Booking.Models;

namespace Event_Ticket_Booking
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
	}
}
