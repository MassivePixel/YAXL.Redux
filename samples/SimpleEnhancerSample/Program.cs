// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using YAXL.Redux;

//using static YAXL.Redux.Store<int>;

namespace SimpleEnhancerSample
{
	class MainClass
	{
		public static Enhancer<T> InitialStateLog<T> (string id)
		 	=> (createStore) => (reducer, initialState, enhancer) =>
			{
				Console.WriteLine ($"{id}: Creating store with type {typeof (T).Name}");
				return createStore (reducer, initialState, enhancer);
			};

		public static void Main (string [] args)
		{
			var store = Store<int>.CreateStore (
				(state, action) => state,
				0,
				Store<int>.Compose (InitialStateLog<int> ("1"), InitialStateLog<int> ("2")));
		}
	}
}
