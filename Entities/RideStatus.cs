using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class RideStatus
	{
		public int Id { get; set; }

			[Required, StringLength(30)]
	public string Name { get; set; } = string.Empty; // New, EnRoute, Completed, Canceled

	[StringLength(200)]
	public string Description { get; set; } = string.Empty;

	public bool IsActive { get; set; } = true;

	public ICollection<Ride> Rides { get; set; } = new List<Ride>();
	}
} 