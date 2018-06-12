using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using DotNetSamples.Core;

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
			using (var client = new HttpClient{ BaseAddress = Registry })
			{
				if (string.IsNullOrEmpty(Name))
				{
					await OutputAvailableSamples(client);
				} 
				else
				{
					await DownloadProject(Name, client);
				}

				return 0;
			}
		}

		private async Task DownloadProject(string name, HttpClient client)
		{
			var sampleData = await client.GetStringAsync("Samples/" + name);
			var sample = JsonConvert.DeserializeObject<Sample>(sampleData);
			var destinationFolder = OutputFolder ?? sample.Name;

			var zipFile = await client.GetStreamAsync(sample.Url);
			using (var zipArchive = new ZipArchive(zipFile))
			{
				try
				{
					zipArchive.ExtractToDirectory(destinationFolder, true);
				}
				catch(Exception exception)
				{
					Console.WriteLine($"An error occured while attempting to extract the {sample.Name} sample.\n{exception.Message}");
				}
			}
		}

		private async Task OutputAvailableSamples(HttpClient client)
		{

			Console.WriteLine($"The available samples from {Registry}");
			Console.WriteLine("---------------------------------------------\n");

			var samplesList = await client.GetStringAsync("");
			var samplesArray = JsonConvert.DeserializeObject<Sample[]>(samplesList);

			Console.WriteLine("Sample Name");
			Console.WriteLine("-----------------------\n");

			foreach (var s in samplesArray)
			{
				Console.WriteLine(s.Name);
			}

			Console.WriteLine($"\nTotal samples found: {samplesArray.Length}");
		}
	}
}