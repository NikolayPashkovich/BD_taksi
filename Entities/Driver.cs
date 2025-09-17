using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Driver
	{
		public int Id { get; set; }

		[Required, StringLength(100)]
		public string FullName { get; set; } = string.Empty;

		[Required, Phone, StringLength(20)]
		public string Phone { get; set; } = string.Empty;

		[Required]
		public DateTime HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		public License? License { get; set; }
		public ICollection<Ride> Rides { get; set; } = new List<Ride>();
		public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
		public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
	}
} 