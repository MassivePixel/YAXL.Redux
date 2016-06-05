// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace YAXL.Redux
{
	public delegate Dispatcher MiddlewareApplier (Dispatcher dispatch);
	public delegate Func<Dispatcher, Dispatcher> Middleware<T> (Store<T> store);

	public partial class Store<T>
	{
		public static Enhancer<T> Compose (params Enhancer<T> [] enhancers)
		{
			if (enhancers == null || enhancers.Length == 0)
				return createStore => createStore;
			if (enhancers.Length == 1)
				return enhancers [0];

			return (createStore) =>
			{
				var last = enhancers [enhancers.Length - 1] (createStore);
				return enhancers.Take (enhancers.Length - 1)
								.Aggregate (last, (acc, enh) => enh (acc));
			};
		}

		public static Func<U, U> Compose<U> (List<Func<U, U>> funcs)
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
			=> (createStore) => (reducer, initialState, enhancer) =>
			{
				var store = createStore (reducer, initialState, enhancer);
				if (middlewares == null || middlewares.Length == 0)
					return store;

				var dispatch = store.Dispatch;

				var chain = middlewares.Select (m => m (store))
									   .ToList ();
				var newDispatch = Compose (chain) (dispatch);

				return store.ReplaceDispatch (newDispatch);
			};
	}
}

