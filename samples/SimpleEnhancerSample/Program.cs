// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using YAXL.Redux;

namespace SimpleEnhancerSample
{
    class MainClass
    {
        static Store<T> Unwrapped<T>(Reducer<T> reducer, T initialState)
        {
            Console.WriteLine($"1: Creating store with type {typeof(T).Name}");

            {
                Console.WriteLine($"2: Creating store with type {typeof(T).Name}");

                {
                    return new Store<T>(reducer, initialState);
                }

                Console.WriteLine($"2: Created store with type {typeof(T).Name}");
            }

            Console.WriteLine($"1: Created store with type {typeof(T).Name}");
        }

        public static Enhancer<T> InitialStateLog<T>(string id)
             => (createStore) => (reducer, initialState, enhancer) =>
            {
                Console.WriteLine($"{id}: Creating store with type {typeof(T).Name}");
                var store = createStore(reducer, initialState, enhancer);
                Console.WriteLine($"{id}: Created store with type {typeof(T).Name}");
                return store;
            };

        public static event EventHandler IncButtonClicked;

        public static void Main(string[] args)
        {
            var s = Store<int>.CreateStore(
                reducer: (state, action) =>
                {
                    if (action.Equals("+"))
                        return state + 1;
                    return state;
                },
                initialState: 0,
                enhancer: (createStore) => (reducer, initialState, enhancer) =>
                {
                    var x = createStore(reducer, initialState, enhancer);

                    IncButtonClicked += (sender, e) =>
                    {
                        x.Dispatch("+");
                    };

                    return x;
                });

            s.Subscribe += (state) =>
            {
                Console.WriteLine($"State changed to {state}");
            };

            IncButtonClicked?.Invoke(null, EventArgs.Empty);
            IncButtonClicked?.Invoke(null, EventArgs.Empty);
            IncButtonClicked?.Invoke(null, EventArgs.Empty);

            Store<int>.CreateStore(
                reducer: (state, action) => state,
                initialState: 0,
                enhancer: Store<int>.Compose(InitialStateLog<int>("1"), InitialStateLog<int>("2")));
        }
    }
}
