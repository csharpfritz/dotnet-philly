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

		public SamplesController(IRepository repo)
		{
			this.Repository = repo;
		}

		public IRepository Repository { get; }

		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<Sample>> Get()
		{
			return Ok(Repository.GetSamples());
		}

		[HttpGet("{command}")]
		public ActionResult<Sample>Get(string command)
		{

			var sample = Repository.Get(command);

			if (sample == null)
			{
				return NotFound();
			}

			return Ok(sample);

		}

	}
}
