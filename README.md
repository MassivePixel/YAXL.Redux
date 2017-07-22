# YAXL.Redux - Yet Another Xamarin Library | Redux
[![Build status](https://ci.appveyor.com/api/projects/status/fb3rycbb6mys2k8e?svg=true)](https://ci.appveyor.com/project/tpetrina/yaxl-redux)
[![NuGet](https://img.shields.io/nuget/v/YAXL.Redux.svg)]()
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)]()

## Intro

YAXL.Redux is a predictable state container for Xamarin and C# applications. It is a straightforward C# port of [redux](https://github.com/reactjs/redux) and aims at solving the exactly same problems.

Let's look at a concrete example:

```csharp
// Actions are defined as actual types
public class IncrementAction {}
public class DecrementAction {}
public class SetCounterAction
{
    public int Value { get; }
    
    public SetCounterAction(int value) { Value = value; }
}

var counter = new Store<int>((state, action) =>
{
    if (action is IncrementAction)
        return state + 1;
    if (action is DecrementAction)
        return state - 1;
    if (action is SetCounterAction s)
        return s.Value;

    return state;
}, 0);
```

This creates a store that acts as a counter since it holds a single `int` value inside. It can handle two string based actions (actions can be any object) which serve as increment/decrement.

We can use it in our Xamarin app for consistent UI updates. If the UI is given as:

```xml
<StackLayout VerticalOptions="Center">
	<Label Text="Counter" />
	<Label x:Name="CounterValue" />
	<Button Clicked="Up_Clicked" Text="+" />
	<Button Clicked="Down_Clicked" Text="-" />
</StackLayout>
```

We would *connect* our store to our UI by projecting the necessary parts. Projection is done when the state is large and when a particular UI part only needs to know about a part of the entire app's state.

```csharp
    public partial class CounterFormsPage : ContentPage
    {
        Store<int> counter;

        public CounterFormsPage()
        {
            InitializeComponent();

            // counter creation ommitted

            counter.Connect(mapStateToProps: state => new
            {
                count = state
            }, propsChanged: props =>
            {
                CounterValue.Text = props.count.ToString();
            });
        }

        void Up_Clicked(object sender, EventArgs e) => counter.Dispatch(new IncrementAction());
        void Down_Clicked(object sender, EventArgs e) => counter.Dispatch(new DecrementAction());
    }
```

Actions don't have to be defined as classes, they can be anything reducer can handle:
```csharp
var counter = new Store<int>((state, action) =>
{
    if (Equals(action, "+"))
        return state + 1;
    if (Equals(action, "-"))
        return state - 1;

    return state;
}, 0);

```

Library can be built from sources or installed via NuGet.

    PM> Install-Package YAXL.Redux -pre
    
## Little bit of extra reading

Blog posts about building this library:

 1. [MVVM, data stores and Redux](http://massivepixel.co/blog/post/mvvm-data-redux)
 2. [Implementing Redux in C#, Part 1](http://massivepixel.co/blog/post/redux-csharp-part1)
 
## Similar libraries

 1. [redux.NET](https://github.com/GuillaumeSalles/redux.NET)
 2. [reducto](https://github.com/pshomov/reducto)

## Pull requests and contributions

This is an open source project and we accept contributes. Please be mindful about style changes when submitting pull requests, we will reject those pull requests which are mostly style changes.

## License

YAXL.Redux is distributed under MIT licence
