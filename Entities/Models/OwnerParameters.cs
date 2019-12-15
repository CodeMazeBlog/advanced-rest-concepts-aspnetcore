using System;

namespace Entities.Models
{
	public class OwnerParameters : QueryStringParameters
	{
		public OwnerParameters()
		{
			OrderBy = "name";
		}

		public uint MinYearOfBirth { get; set; }
		public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;

		public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;

		public string Name { get; set; }
	}
}
