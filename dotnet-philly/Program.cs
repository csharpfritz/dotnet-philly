using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetSamples.Core;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotnet_philly
{
	[Command(Description = "Download and extract sample code from Microsoft's Sample Repository")]
	class Program
	{

		// TODO: Need to be able to configure registry in the future
		internal static readonly Uri Registry = new Uri("https://localhost:5001/Samples");


		public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

		[Argument(0, Description = "Name of the sample project to fetch")]
		public string Name { get; }

		[Argument(1, Description ="Folder to write the sample to.  If not specified, then the name of the sample as a child of the current directory")]
		public string OutputFolder { get; }

		//[Option(Description = "An optional parameter, with a default value.\nThe number of times to say hello.")]
		//[Range(1, 1000)]
		//public int Count { get; } = 1;

		private async Task<int> OnExecute()
		{

			using (var client = new HttpClient())
			{

				client.BaseAddress = Registry;

				if (string.IsNullOrEmpty(Name))
				{
					await OutputAvailableSamples(client);
					return 0;
				} else
				{
					await DownloadProject(Name, client);
					return 0;
				}





			}

			return 0;
		}

		private async Task DownloadProject(string name, HttpClient client)
		{

			var sampleData = await client.GetStringAsync("Samples/" + name);
			var sample = JsonConvert.DeserializeObject<Sample>(sampleData);

			var destinationFolder = CreateOutputFolder(sample);

			var zipFile = await client.GetStreamAsync(sample.Url);
			using (var zipArchive = new ZipArchive(zipFile))
			{
				zipArchive.ExtractToDirectory(destinationFolder);
			}

		}

		private string CreateOutputFolder(Sample sample)
		{

			var destinationFolder = string.IsNullOrEmpty(OutputFolder) ? sample.Name : OutputFolder;
			if (!Directory.Exists(destinationFolder))
			{
				// Need to handle errors here 
				Directory.CreateDirectory(destinationFolder);
			}

			return destinationFolder;
		}

		private async Task OutputAvailableSamples(HttpClient client)
		{

			Console.WriteLine($"The available samples from {Registry}");
			Console.WriteLine("---------------------------------------------");
			Console.WriteLine("");

			var samplesList = await client.GetStringAsync("");
			var samplesArray = JsonConvert.DeserializeObject<Sample[]>(samplesList);

			Console.WriteLine("Sample Name");
			Console.WriteLine("-----------------------");
			Console.WriteLine("");

			foreach (var s in samplesArray)
			{

				Console.WriteLine(s.Name);

			}

			Console.WriteLine("");

			Console.WriteLine($"Total samples found: {samplesArray.Length}");

		}



	}
}
