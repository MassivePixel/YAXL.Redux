// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using YAXL.Redux;

namespace ComposeReducers
{
    public class CombinedStore
    {
        public int Counter1 { get; set; }
        public int Counter2 { get; set; }
    }

    public class CombinedStore2
    {
        public CombinedStore Store1 { get; set; }
        public CombinedStore Store2 { get; set; }
        public int Counter1 { get; set; }

        public static CombinedStore2 Reducer(CombinedStore2 state, object action)
            => new CombinedStore2
            {
            };
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Func<int, object, int> reducer = (state, action) => state;
            Func<CombinedStore, object, CombinedStore> reducer2 = (state, action) => state;

            // if state becomes to large for a single reducer, one can always
            // create a meta reducer that will combine separate reducer-based
            // stores into a large state
            var store = Store<CombinedStore>.CreateStore(
                (state, action) => new CombinedStore
                {
                    Counter1 = reducer(state.Counter1, action),
                    Counter2 = reducer(state.Counter2, action)
                },
                new CombinedStore());

            // recursion!
            // no matter how complex your state is, you can always subdivide it
            var store2 = new Store<CombinedStore2>((state, action) =>
                new CombinedStore2
                {
                    Store1 = reducer2(state?.Store1, action),
                    Store2 = reducer2(state?.Store2, action),
                    Counter1 = reducer(state?.Counter1 ?? 0, action)
                });

            // to simplify the code above, reducer can be brought from elsewhere
            var store3 = new Store<CombinedStore2>(CombinedStore2.Reducer);
            var store4 = Store.Create(CombinedStore2.Reducer, new CombinedStore2());

            store.Dispatch(null);
        }
    }

    public static class Store
    {
        public static Store<T> Create<T>(Reducer<T> reducer, T initialState) => new Store<T>(reducer, initialState);
    }
}
