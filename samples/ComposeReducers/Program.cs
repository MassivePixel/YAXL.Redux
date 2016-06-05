using System;

namespace ComposeReducers
{
	public class CombinedStore
	{
		public int Counter1 { get; set; }
		public int Counter2 { get; set; }
	}

	class MainClass
	{
		public static void Main(string[] args)
		{
			Func<int, object, int> reducer = (state, action) => state;

			var store = YAXL.Redux.Store<CombinedStore>.CreateStore(
				(state, action) => new CombinedStore
				{
					Counter1 = reducer(state.Counter1, action),
					Counter2 = reducer(state.Counter2, action)
				},
				new CombinedStore
				{
					Counter1 = 0,
					Counter2 = 0
				});

			store.Dispatch(null);
		}
	}
}
