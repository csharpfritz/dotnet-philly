using DotNetSamples.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetSamples.Web
{
	public class Repository : IRepository
	{

		private Sample[] _Samples = new Sample[] {

				new Sample
				{
					Url="https://dot.net/MusicStore.zip",
					Command="musicstore",
					Name="Music Store MVC",
					Description="The Music Store MVC Sample",
					FrameworkVersion="aspnetcore21"
				},
				new Sample
				{
					Url="https://dot.net/NerdDinner.zip",
					Command="nerddinner",
					Name="NerdDinner MVC",
					Description="The infamous NerdDinner application... its like Meetup except for Nerds and Tacos",
					FrameworkVersion="aspnetcore20"
				}

			};


		public IEnumerable<Sample> GetSamples()
		{

			return _Samples;

		}

		public Sample Get(string command)
		{

			return _Samples.FirstOrDefault(s => s.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase));

		}

	}
}
