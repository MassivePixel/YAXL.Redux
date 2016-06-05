// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using YAXL.Redux;

namespace SmartActionsSample
{
	public class SimpleReducer<T>
	{
		Dictionary<Type, Delegate> handlers = new Dictionary<Type, Delegate> ();

		public SimpleReducer<T> When<U> (Func<T, U, T> handler)
		{
			handlers.Add (typeof (U), handler);
			return this;
		}

		public Reducer<T> Compile ()
		{
			return (state, action) =>
			{
				Delegate handler;
				if (handlers.TryGetValue (action.GetType(), out handler))
					return (T)handler.DynamicInvoke (state, action);
				return state;
			};
		}
	}

	class MainClass
	{
		public static void Main (string [] args)
		{
			var store = new Store<int> (
				new SimpleReducer<int> ()
				.When<int> ((state, action) =>
				{
					return state;
				})
				.When<string> ((state, action) =>
				{
					return state;
				})
				.Compile (),
				0);

			store.Dispatch (1);
			store.Dispatch ("");

			Console.ReadKey ();
		}
	}
}
