using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class VehicleModel
	{
		public int Id { get; set; }

		[Required, StringLength(50)]
		public string Name { get; set; } = string.Empty;

		[Required, StringLength(20)]
		public string Class { get; set; } = "Economy"; // Economy/Comfort/Business

		[Required]
		public int MakeId { get; set; }
		public VehicleMake? Make { get; set; }

		public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
	}
} 