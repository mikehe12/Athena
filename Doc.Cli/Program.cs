using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Cli
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var currentFolder = Directory.GetCurrentDirectory();
			var targetFolder = Path.Combine(currentFolder, args[0]);

			//var files = Directory.EnumerateFiles(targetFolder, "*.cs", SearchOption.AllDirectories);

			//var files = new FileInfo[] { new FileInfo(args[0])};

			foreach(var filename in args)
			{
				Console.WriteLine($"Parsing file: {filename}");
				Console.WriteLine();

				using(var fileStream = new FileStream(filename, FileMode.Open))
				//using(var reader = new StreamReader(fileStream, Encoding.UTF8))
				{
					var reader = PipeReader.Create(fileStream);

					while (true)
					{
						var result = await reader.ReadAsync();
						var buffer = result.Buffer;

						while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
						{
							// Process the line.
							ProcessLine(line);
						}

						// Tell the PipeReader how much of the buffer has been consumed.
						reader.AdvanceTo(buffer.Start, buffer.End);

						// Stop reading if there's no more data coming.
						if (result.IsCompleted)
						{
							break;
						}
					}

					//string.Create()
					//	Encoding.ASCII.GetChars()
					//	Span

				}

				//	var start = file.AsSpan().IndexOf('$');
				//if (start > 0) Console.WriteLine($"{file},{start}");
			}


			Console.ReadKey();

			bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
			{
				// Look for a EOL in the buffer.
				SequencePosition? position = buffer.PositionOf((byte)'$');

				if (position == null)
				{
					line = default;
					return false;
				}

				// Skip the line + the \n.
				line = buffer.Slice(0, position.Value);
				buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
				return true;
			}

			void ProcessLine(in ReadOnlySequence<byte> buffer)
			{
				foreach (var segment in buffer)
				{
					Console.Write(Encoding.UTF8.GetString(segment.Span));
				}
				Console.WriteLine();
			}
		}
	}
}
