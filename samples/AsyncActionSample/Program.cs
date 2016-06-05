// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using YAXL.Redux;

namespace AsyncActionSample
{
	public delegate Task AsyncAction<T> (T state, Dispatcher dispatch);

	public static class StoreExtensions
	{
		public static void DispatchAsync<T> (this Store<T> store, AsyncAction<T> asyncAction)
		{
			store.Dispatch (asyncAction);
		}
	}

	class MainClass
	{
		public static Func<Dispatcher, Dispatcher> Thunk<T> (Store<T> store)
			=> next => action =>
			{
				if (action is AsyncAction<T>)
				{
					return ((AsyncAction<T>)action) (store.State, store.Dispatch);
				}
				return next (action);
			};

		public static Func<Dispatcher, Dispatcher> Lock<T> (Store<T> store)
			=> next => action =>
			{
				lock (store)
					return next (action);
			};

		public static void Main (string [] args)
		{
			var store = Store<int>.CreateStore ((state, action) =>
			{
				Console.WriteLine ($"{action.GetType ().FullName}");

				if (action is string && (string)action == "+")
					return state + 1;

				return state;
			}, 0, Store<int>.ApplyMiddleware (Thunk));

			store.Subscribe += (state) => Console.WriteLine ($"State is {state}");

			store.Dispatch ((AsyncAction<int>)(async (state, dispatch) =>
			{
				await Task.Delay (TimeSpan.FromSeconds (2));
				dispatch ("+");
			}));

			store.DispatchAsync (async (state, dispatch) =>
			 {
				 await Task.Delay (TimeSpan.FromSeconds (3));
				 dispatch ("+");
			 });

			Console.ReadKey ();
		}
	}
}
