using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class VehicleMake
	{
		public int Id { get; set; }

		[Required, StringLength(50)]
		public string Name { get; set; } = string.Empty;

		public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
	}
} 