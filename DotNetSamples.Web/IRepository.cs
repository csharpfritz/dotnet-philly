using DotNetSamples.Core;
using System.Collections.Generic;

namespace DotNetSamples.Web
{
	public interface IRepository
	{

		IEnumerable<Sample> GetSamples();

		Sample Get(string command);

	}
}