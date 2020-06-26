using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace RxSequenceProcessing
{
	public static class ObservableExtensions
	{
		public static IDisposable Processing<T>(this IObservable<T> source, Func<T[], Task> callback)
		{
			var closer = new Subject<Unit>();
			var prevTask = Task.CompletedTask;

			return source
				.Window(() => closer)
				.Select(window =>
				{
					var tcs = new TaskCompletionSource<object>();
					var currentTask = tcs.Task;

					window.FirstAsync()
						.Subscribe(async _ =>
						{
							await prevTask.ConfigureAwait(false);

							prevTask = currentTask;
							closer.OnNext(Unit.Default);
						});
					return (window, tcs);
				})
				.Subscribe(tp =>
				{
					tp.window
						.ToArray()
						.Subscribe(async n =>
						{
							await callback(n).ConfigureAwait(false);
							tp.tcs.SetResult(true);
						});
				});


		}
	}
}
