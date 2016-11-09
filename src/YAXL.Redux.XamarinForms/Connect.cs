// Copyright (c) Massive Pixel.  All Rights Reserved.  Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace YAXL.Redux.XamarinForms
{
    public static class StoreExtensions
    {
        public class StoreMapper<TState, TProps>
        {
            public Store<TState> Store { get; }
            public Func<TState, TProps> MapStateToProps { get; }

            public StoreMapper(Store<TState> store, Func<TState, TProps> mapStateToProps)
            {
                if (store == null)
                    throw new ArgumentNullException(nameof(store));
                if (mapStateToProps == null)
                    throw new ArgumentNullException(nameof(mapStateToProps));

                Store = store;
                MapStateToProps = mapStateToProps;
            }
        }

        class Connection<TState, TProps> : IDisposable
        {
            readonly Store<TState> store;
            readonly Func<TState, TProps> mapStateToProps;

            readonly Store<TState>.Subscriber subscription;
            readonly Action<TProps> propsChanged;

            TProps lastProps;

            public Connection(Store<TState> store, Func<TState, TProps> mapStateToProps, Action<TProps> propsChanged)
            {
                if (store == null)
                    throw new ArgumentNullException(nameof(store));
                if (mapStateToProps == null)
                    throw new ArgumentNullException(nameof(mapStateToProps));
                if (propsChanged == null)
                    throw new ArgumentNullException(nameof(propsChanged));

                this.store = store;
                this.mapStateToProps = mapStateToProps;
                this.propsChanged = propsChanged;

                lastProps = mapStateToProps(store.State);
                SetProps(lastProps);

                subscription = (state) =>
                {
                    var newProps = mapStateToProps(state);
                    if (!lastProps.Equals(newProps))
                    {
                        SetProps(newProps);
                        lastProps = newProps;
                    }
                };

                store.Subscribe += subscription;
            }

            void SetProps(TProps props) => propsChanged?.Invoke(props);
            void Cleanup() => store.Subscribe -= subscription;

            public void Dispose()
            {
                store.Subscribe -= subscription;
                GC.SuppressFinalize(this);
            }
        }

        public static IDisposable To<TState, TProps>(this StoreMapper<TState, TProps> connector, Action<TProps> propsChanged)
        => new Connection<TState, TProps>(connector.Store, connector.MapStateToProps, propsChanged);

        class VisualElementConnection<TState, TProps> : IDisposable
        {
            readonly Store<TState> store;
            readonly Func<TState, TProps> mapStateToProps;
            readonly Action<TProps> propsChanged;
            readonly Element element;

            TProps lastProps;
            Page page;

            public VisualElementConnection(Store<TState> store, Element element, Func<TState, TProps> mapStateToProps, Action<TProps> propsChanged)
            {
                if (store == null)
                    throw new ArgumentNullException(nameof(store));
                if (element == null)
                    throw new ArgumentNullException(nameof(element));
                if (mapStateToProps == null)
                    throw new ArgumentNullException(nameof(mapStateToProps));
                if (propsChanged == null)
                    throw new ArgumentNullException(nameof(propsChanged));

                this.store = store;
                this.element = element;
                this.mapStateToProps = mapStateToProps;
                this.propsChanged = propsChanged;

                FindPage();
                element.PropertyChanged += Element_PropertyChanged;
            }

            ~VisualElementConnection()
            {
                Cleanup();
            }

            void FindPage()
            {
                var p = element.Parent;
                while (p != null)
                {
                    if (p is Page && p != page)
                    {
                        page = (Page)p;
                        page.Appearing += Page_Appearing;
                        page.Disappearing += Page_Disappearing;
                        break;
                    }
                    p = p.Parent;
                }
            }

            void OnStateChange(TState state)
            {
                var newProps = mapStateToProps(state);
                if (!lastProps.Equals(newProps))
                {
                    propsChanged?.Invoke(newProps);
                    lastProps = newProps;
                }
            }

            void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Element.Parent))
                {
                    if (element.Parent == null)
                    {
                        UnsubPage();
                    }
                    else
                    {
                        var notify = page == null;
                        FindPage();
                        if (notify && page != null)
                            OnAppearing();
                    }
                }
            }

            void UnsubPage()
            {
                if (page != null)
                {
                    page.Appearing -= Page_Appearing;
                    page.Disappearing -= Page_Disappearing;
                    page = null;
                }
            }

            void Page_Appearing(object sender, EventArgs e) => OnAppearing();

            void OnAppearing()
            {
                OnStateChange(store.State);

                store.Subscribe -= OnStateChange;
                store.Subscribe += OnStateChange;
                element.PropertyChanged -= Element_PropertyChanged;
                element.PropertyChanged += Element_PropertyChanged;
            }

            void Page_Disappearing(object sender, EventArgs e)
            {
                store.Subscribe -= OnStateChange;
            }

            public void Dispose() => Cleanup();

            void Cleanup()
            {
                UnsubPage();
                element.PropertyChanged -= Element_PropertyChanged;
                store.Subscribe -= OnStateChange;
            }
        }

        public static StoreMapper<TState, TProps> Connect<TState, TProps>(this Store<TState> store, Func<TState, TProps> mapStateToProps)
        => new StoreMapper<TState, TProps>(store, mapStateToProps);

        public static IDisposable To<TState, TProps, TElement>(this StoreMapper<TState, TProps> connector, TElement ve, Action<TProps> propsChanged)
            where TElement : VisualElement
        => new VisualElementConnection<TState, TProps>(connector.Store, ve, connector.MapStateToProps, propsChanged);
    }
}
