using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Vehicle
	{
		public int Id { get; set; }

		[Required, StringLength(12)]
		public string PlateNumber { get; set; } = string.Empty;

		[Required, StringLength(17, MinimumLength = 11)]
		public string Vin { get; set; } = string.Empty;

		[Range(1990, 2100)]
		public int Year { get; set; }

		[StringLength(20)]
		public string Color { get; set; } = string.Empty;

		public bool IsActive { get; set; } = true;

		[Required]
		public int ModelId { get; set; }
		public VehicleModel? Model { get; set; }

		public ICollection<Ride> Rides { get; set; } = new List<Ride>();
		public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
		public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
		public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
	}
} 