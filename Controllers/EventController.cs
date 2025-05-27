using Event_Ticket_Booking.Data;
using Event_Ticket_Booking.DTO;
using Event_Ticket_Booking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Ticket_Booking.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public EventController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(
	    [FromQuery] string eventCategory,
	    [FromQuery] string sortBy = "eventDate",
	    [FromQuery] string sortDirection = "asc",
	    [FromQuery] int pageNumber = 1,
	    [FromQuery] int pageSize = 10)
		{
			var query = _context.Bookings
				.Include(b => b.TicketTier)
				.ThenInclude(t => t.Event)
				.Include(b => b.User)
				.AsQueryable();

			// 🔹 Filtering by Event Category
			if (!string.IsNullOrEmpty(eventCategory))
			{
				query = query.Where(b => b.TicketTier.Event.Category == eventCategory);
			}

			// 🔹 Sorting Logic
			query = sortBy switch
			{
				"eventName" => sortDirection == "asc" ? query.OrderBy(b => b.TicketTier.Event.Name) : query.OrderByDescending(b => b.TicketTier.Event.Name),
				"eventDate" => sortDirection == "asc" ? query.OrderBy(b => b.TicketTier.Event.EventDate) : query.OrderByDescending(b => b.TicketTier.Event.EventDate),
				_ => query
			};

			// 🔹 Pagination
			var totalRecords = await query.CountAsync();
			var bookings = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(b => new BookingDto
				{
					UserName = b.User.Name,
					EventName = b.TicketTier.Event.Name,
					EventDate = b.TicketTier.Event.EventDate,
					TicketTier = b.TicketTier.TierName,
					Price = b.TicketTier.Price,
					TicketQuantity = b.TicketQuantity
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = bookings });
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<BookingDto>> GetBookingById(int id)
		{
			var booking = await _context.Bookings
				.Include(b => b.TicketTier)
				.ThenInclude(t => t.Event)
				.Include(b => b.User)
				.Where(b => b.Id == id)
				.Select(b => new BookingDto
				{
					UserName = b.User.Name,
					EventName = b.TicketTier.Event.Name,
					EventDate = b.TicketTier.Event.EventDate,
					TicketTier = b.TicketTier.TierName,
					Price = b.TicketTier.Price,
					TicketQuantity = b.TicketQuantity
				})
				.FirstOrDefaultAsync();

			return booking == null ? NotFound(new { message = "Booking not found" }) : Ok(booking);
		}

		[HttpPost]
		public async Task<ActionResult<Booking>> AddBooking(CreateBookingDto bookingDto)
		{
			var eventExists = await _context.Events.AnyAsync(e => e.Id == bookingDto.EventId);
			var userExists = await _context.Users.AnyAsync(u => u.Id == bookingDto.UserId);
			var ticketTier = await _context.TicketTiers
				.FirstOrDefaultAsync(t => t.Id == bookingDto.TicketTierId && t.EventId == bookingDto.EventId);

			if (!eventExists || !userExists || ticketTier == null)
			{
				return BadRequest(new { message = "Invalid EventId, UserId, or TicketTierId" });
			}

			if (ticketTier.AvailableTickets < bookingDto.TicketQuantity)
			{
				return BadRequest(new { message = "Not enough tickets available for this tier." });
			}

			// Deduct booked tickets
			ticketTier.AvailableTickets -= bookingDto.TicketQuantity;

			var booking = new Booking
			{
				TicketTierId = bookingDto.TicketTierId,
				UserId = bookingDto.UserId,
				TicketQuantity = bookingDto.TicketQuantity,
				BookingDate = DateTime.Now
			};

			_context.Bookings.Add(booking);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBooking(int id, CreateBookingDto bookingDto)
		{
			var booking = await _context.Bookings.FindAsync(id);
			if (booking == null)
			{
				return NotFound(new { message = "Booking not found" });
			}

			// Prevent updating past bookings
			if (booking.TicketTier.Event.EventDate < DateTime.Now)
			{
				return BadRequest(new { message = "Past bookings cannot be modified." });
			}

			booking.TicketQuantity = bookingDto.TicketQuantity;
			await _context.SaveChangesAsync();
			return Ok(new { message = "Booking updated successfully" });
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> CancelBooking(int id)
		{
			var booking = await _context.Bookings.FindAsync(id);
			if (booking == null)
			{
				return NotFound(new { message = "Booking not found" });
			}

			var ticketTier = await _context.TicketTiers.FindAsync(booking.TicketTierId);
			ticketTier.AvailableTickets += booking.TicketQuantity;

			_context.Bookings.Remove(booking);
			await _context.SaveChangesAsync();
			return Ok(new { message = "Booking canceled and tickets refunded." });
		}
	}
}
