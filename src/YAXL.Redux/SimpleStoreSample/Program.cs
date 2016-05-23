// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using YAXL.Redux;

namespace SimpleStoreSample
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			var store = new Store<int> ((state, action) =>
			{
				if (action is int) return (int)action;
				if (action is string && (string)action == "+") return state + 1;
				if (action is string && (string)action == "-") return state - 1;

				return state;
			}, 10);
			Console.WriteLine ($"Initial state = {store.State}");

			store.Subscribe += number => Console.WriteLine ($"State changed to {number}");
			store.Dispatch ("+");
			store.Dispatch ("-");
			store.Dispatch (8);
			store.Dispatch ("-");
			store.Dispatch ("-");

			Middleware<int> m = (state, dispatch) => next => action =>
			{
				Console.WriteLine ("Hello");
				var result = next (action);
				return result;
			};

			var store2 = Store<int>.CreateStore ((state, action) => state, 0,
												 Store<int>.ApplyMiddleware (m));

			store2.Dispatch (null);
		}
	}
}
