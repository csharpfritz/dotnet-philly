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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotnet_philly
{
	[Command(Description = "Download and extract sample code from Microsoft's Sample Repository")]
	class Program
	{

		// TODO: Need to be able to configure registry in the future
		internal static readonly Uri Registry = new Uri("https://localhost:5001/Samples");

        private readonly ILogger _logger;


		public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        public Program()
        {
            _logger = new LoggerFactory()
                    .AddConsole(LogLevel.Information)
                    .AddDebug(LogLevel.Debug)
                    .CreateLogger<Program>();
        }

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
                    _logger.LogInformation($"Retrieving samples from '{Registry}'");
					await OutputAvailableSamples(client);
					return 0;
				} else
				{
                    _logger.LogInformation($"Downloading sample '{Name}' from '{Registry}'");
					await DownloadProject(Name, client);
					return 0;
				}

			}

		}

		private async Task DownloadProject(string name, HttpClient client)
		{

            var sampleData = string.Empty;

            try
            {
                sampleData = await client.GetStringAsync("Samples/" + name);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Could not download sample '{Name}'. {ex.Message}");
                return;
            }

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
            var samplesList = string.Empty;
            try
            {
                samplesList = await client.GetStringAsync("");
            }
            catch (HttpRequestException ex)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not retrieve samples from {Registry}.\n\r{ex.Message}");
                Console.ForegroundColor = color;
                return;
            }

            Console.WriteLine($"The available samples from {Registry}");
			Console.WriteLine("---------------------------------------------");
			Console.WriteLine("");

            
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
