using System;
using System.Threading.Tasks;
using YAXL.Redux;

namespace AsyncActionSample
{
	public delegate Task AsyncAction (DispatchDelegate dispatch);

	class MainClass
	{
		public static void Main (string [] args)
		{
			var store = Store<int>.CreateStore ((state, action) =>
			{
				Console.WriteLine ($"{action.GetType ().FullName}");

				if (action is string && (string)action == "+")
					return state + 1;
				
				return state;
			}, 0);

			store.Subscribe += (state) => Console.WriteLine ($"State is {state}");

			store.Dispatch ((AsyncAction)(async (DispatchDelegate dispatch) =>
			{
				await Task.Delay (TimeSpan.FromSeconds (2));
				dispatch ("+");
			}));
		}
	}
}
