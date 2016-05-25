// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace YAXL.Redux
{
	using ActionType = System.Object;

	public delegate ActionType DispatchDelegate (ActionType action);
	public delegate T Reducer<T> (T state, ActionType action);

	public delegate Store<T> CreateStoreDelegate<T> (Reducer<T> reducer, T initialState, Enhancer<T> enhancer);
	public delegate Func<Reducer<T>, T, Store<T>> Enhancer<T> (CreateStoreDelegate<T> createStore);

	public partial class Store<T>
	{
		bool isDispatching;
		Reducer<T> reducer;

		public T State { get; private set; }
		public DispatchDelegate Dispatch { get; private set; }

		public delegate void SubscribeDelegate (T state);
		public event SubscribeDelegate Subscribe;

		public Store (Reducer<T> reducer, T initialState)
		{
			this.reducer = reducer;
			State = initialState;
			Dispatch = DoDispatch;
		}

		private Store (Store<T> other)
		{
			this.reducer = other.reducer;
			this.State = other.State;
			this.Dispatch = other.Dispatch;
		}

		ActionType DoDispatch (ActionType action)
		{
			if (isDispatching)
				throw new InvalidOperationException ("Cannot dispatch while dispatching");

			try
			{
				isDispatching = true;
				State = reducer (State, action);
			}
			finally
			{
				isDispatching = false;
			}

			Subscribe?.Invoke (State);

			return action;
		}

		public Store<T> WithDispatch (DispatchDelegate dispatch)
			=> new Store<T> (this)
			{
				Dispatch = dispatch
			};

		public static Store<T> CreateStore (Reducer<T> reducer, T initialState, Enhancer<T> enhancer = null)
		{
			if (enhancer != null)
				return enhancer (CreateStore) (reducer, initialState);

			return new Store<T> (reducer, initialState);
		}
	}
}
