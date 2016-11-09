// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace YAXL.Redux
{
    public partial class Store<T>
    {
        class Unsubscriber : IDisposable
        {
            Action unsubscribeAction;

            public Unsubscriber(Action unsubscribeAction)
            {
                if (unsubscribeAction == null)
                    throw new ArgumentNullException(nameof(unsubscribeAction));

                this.unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                unsubscribeAction?.Invoke();
                unsubscribeAction = null;
            }
        }

        public IDisposable Connect<TProps>(Func<T, TProps> mapStateToProps, Action<TProps> propsChanged)
        {
            if (mapStateToProps == null)
                throw new ArgumentNullException(nameof(mapStateToProps));
            if (propsChanged == null)
                throw new ArgumentNullException(nameof(propsChanged));

            var lastProps = mapStateToProps(State);
            propsChanged(lastProps);

            Subscriber subscription = (state) =>
            {
                var newProps = mapStateToProps(state);
                if (!lastProps.Equals(newProps))
                {
                    propsChanged?.Invoke(newProps);
                    lastProps = newProps;
                }
            };

            Subscribe += subscription;

            return new Unsubscriber(() => Subscribe -= subscription);
        }
    }
}

