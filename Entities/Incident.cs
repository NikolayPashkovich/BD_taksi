using System;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Incident
	{
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required, StringLength(200)]
		public string Description { get; set; } = string.Empty;

		[Range(0, 100000000)]
		public decimal DamageCost { get; set; }

		[Required]
		public int VehicleId { get; set; }
		public Vehicle? Vehicle { get; set; }

		[Required]
		public int DriverId { get; set; }
		public Driver? Driver { get; set; }
	}
} 