using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotNetSamples.Core;

namespace DotNetSamples.Web
{
	public class Repository : IRepository
	{
		private Sample[] _Samples = new Sample[] {
				new Sample
				{
					Url="https://github.com/aspnet/MusicStore/archive/dev.zip",
					Command="musicstore",
					Name="Music Store MVC",
					Description="The Music Store MVC Sample",
					FrameworkVersion="aspnetcore21"
				},
				new Sample
				{
					Url="https://github.com/aspnet/NerdDinner/archive/master.zip",
					Command="nerddinner",
					Name="NerdDinner MVC",
					Description="The infamous NerdDinner application... its like Meetup except for Nerds and Tacos",
					FrameworkVersion="aspnetcore20"
				}
			};

		public IEnumerable<Sample> GetSamples() => _Samples;

		public Sample Get(string command) => _Samples.FirstOrDefault(s => s.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase)); 
	}
}
