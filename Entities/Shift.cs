using System;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Shift
	{
		public int Id { get; set; }

		[Required]
		public DateTime Start { get; set; }

		public DateTime? End { get; set; }

		[Range(0, 2000)]
		public double DistanceKm { get; set; }

		[Required]
		public int DriverId { get; set; }
		public Driver? Driver { get; set; }

		[Required]
		public int VehicleId { get; set; }
		public Vehicle? Vehicle { get; set; }
	}
} 