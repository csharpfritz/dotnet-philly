using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DotNetSamples.Core;
using Microsoft.Extensions.DependencyInjection;
using dotnet_philly.Startup;
using Microsoft.Extensions.Configuration;
using dotnet_philly.Attributes;

namespace dotnet_philly
{
	[Command(Description = "Download and extract sample code from Microsoft's Sample Repository")]
	class Program
	{
		internal Uri Registry = Configuration.GetValue<Uri>("RegistryUri");

		private readonly ILogger _logger;

		public static int Main(string[] args)
		{
			Configuration = ConfigureConfiguration.Execute();

			var services = ConfigureServices.Execute(new ServiceCollection(), Configuration);

			var app = new CommandLineApplication<Program>();
			app.Conventions
					.UseDefaultConventions()
					.UseConstructorInjection(services);

			return app.Execute(args);
		}

		public static IConfiguration Configuration { get; private set; }

		public Program(ILogger<Program> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Pause the Console when attached to a debugger.
		/// This will allow us to examine the Console output prior to the window closing.
		/// </summary>
		/// <param name="msg"></param>
		private void PauseDebugger(string msg = "Press any key to continue...")
		{
			if (!System.Diagnostics.Debugger.IsAttached)
				return;

			Console.WriteLine("\n\n\n");
			Console.Write(msg);
			Console.ReadKey();
		}

		[Argument(0, Description = "Name of the sample project to fetch")]
		public string Name { get; }

		[Option(Template = "-o|--output <path>", Description = "Location to place the Sample output. If no output is specified, the current directory is used.")]
		public string OutputFolder { get; }

		[IsValidUrl]
		[Option(Template = "-s|--sample-source <url>", Description = "Specifies a custom Samples catalog to use.")]
		public string SampleSource { 
			get
			{
				return Registry.ToString();
			}
			set {
				Registry = new Uri(value);
			}
		}

		[Option(Description = "Show details of the sample.")]
		public bool Details { get; }

		private async Task<int> OnExecute()
		{

			using (var client = new HttpClient { BaseAddress = Registry })
			{
				if (Details)
				{
					_logger.LogInformation($"Retrieving sample '{Name}' details from '{Registry}'");
					await OutputSampleDetails(Name, client);
				}
				else if (string.IsNullOrEmpty(Name))
				{
					_logger.LogInformation($"Retrieving samples from '{Registry}'");
					await OutputAvailableSamples(client);
				}
				else
				{
					_logger.LogInformation($"Downloading sample '{Name}' from '{Registry}'");
					await DownloadProject(Name, client);
				}
			}

			// Wait before exiting if attached to a debugger.
			PauseDebugger();

			return 0;
		}

		private async Task DownloadProject(string name, HttpClient client)
		{

			var sample = await GetSampleAsync(name, client);
			if (sample == null)
				return;

			var destinationFolder = OutputFolder ?? sample.Name;

			var zipFile = await client.GetStreamAsync(sample.Url);
			using (var zipArchive = new ZipArchive(zipFile))
			{
				try
				{
					zipArchive.ExtractToDirectory(destinationFolder, true);
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An error occured while attempting to extract the {sample.Name} sample.\n{exception.Message}");
				}
			}
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
			Console.WriteLine("---------------------------------------------\n");

			var samplesArray = JsonConvert.DeserializeObject<Sample[]>(samplesList);

			var lineFormat = "{0,-20}  {1,-10}  {2,-50}";
			Console.WriteLine(lineFormat, "Sample Name", "Command", "Description");
			Console.WriteLine(new String('-', 84) + "\n");

			foreach (var s in samplesArray)
			{
				Console.WriteLine(lineFormat, s.Name, s.Command, s.Description.Substring(0, s.Description.Length > 50 ? 49 : s.Description.Length - 1));
			}

			Console.WriteLine($"\nTotal samples found: {samplesArray.Length}");
		}

		private async Task OutputSampleDetails(string name, HttpClient client)
		{
			var sample = await GetSampleAsync(name, client);
			if (sample == null)
				return;

			var lineFormat = "{0,-15} {1}";
			Console.WriteLine(lineFormat, "Name:", sample.Name);
			Console.WriteLine(lineFormat, "Command:", sample.Command);
			Console.WriteLine(lineFormat, "Url:", sample.Url);
			Console.WriteLine(lineFormat, "Description:", sample.Description);
		}

		private async Task<Sample> GetSampleAsync(string name, HttpClient client)
		{
			var sampleData = string.Empty;

			try
			{
				sampleData = await client.GetStringAsync("Samples/" + name);
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError($"Could not download sample '{Name}'. {ex.Message}");
				return null;
			}

			return JsonConvert.DeserializeObject<Sample>(sampleData);
		}
	}
}