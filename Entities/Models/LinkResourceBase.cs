using System.Collections.Generic;

namespace Entities.Models
{
	public class LinkResourceBase
	{
		public LinkResourceBase()
		{

		}

		public List<Link> Links { get; set; } = new List<Link>();
	}
}
