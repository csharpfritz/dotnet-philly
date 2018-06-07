using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetSamples.Core;
using Microsoft.AspNetCore.Mvc;

namespace DotNetSamples.Web.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class SamplesController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<Sample>> Get()
		{
			return new Sample[] {

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
		}

	}
}
