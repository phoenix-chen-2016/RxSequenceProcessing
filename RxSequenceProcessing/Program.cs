using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RxSequenceProcessing
{
	class Program
	{
		static void Main(string[] args)
		{
			var source = Observable.Interval(TimeSpan.FromMilliseconds(100));

			var rand = new Random();

			source
				.Processing(async n =>
				{
					Console.WriteLine("{0}", string.Join(",", n));

					await Task.Delay(rand.Next(100, 5001)).ConfigureAwait(false);
				});

			Console.ReadKey();
		}
	}
}
