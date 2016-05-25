using System.Collections.Generic;
using System.Linq;

namespace YAXL.Redux
{
	public delegate DispatchDelegate MiddlewareApplier (DispatchDelegate app);
	public delegate MiddlewareApplier Middleware<T> (Func<T> state, DispatchDelegate dispatch);

	public partial class Store<T>
	{
		public static MiddlewareApplier Compose (List<MiddlewareApplier> funcs)
		{
			if (funcs == null || funcs.Count == 0)
				return dispatch => dispatch;
			if (funcs.Count == 1)
				return d => funcs [0] (d);

			return d =>
			{
				var action = funcs [funcs.Count - 1] (d);
				return funcs.Take (funcs.Count - 1)
							.Aggregate (action, (arg1, arg2) => arg2 (arg1));
			};
		}

		public static Enhancer<T> ApplyMiddleware (params Middleware<T> [] middlewares)
		{
			return (createStore) => (reducer, initialState) =>
			{
				var store = createStore (reducer, initialState, null);
				if (middlewares == null || middlewares.Length == 0)
					return store;

				var dispatch = store.Dispatch;

				var chain = middlewares.Select (m => m (() => store.State, dispatch))
									   .ToList ();
				var newDispatch = Compose (chain) (dispatch);

				return store.WithDispatch (newDispatch);
			};
		}
	}
}

