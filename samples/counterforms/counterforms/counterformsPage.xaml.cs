using System;
using Xamarin.Forms;
using YAXL.Redux;

namespace counterforms
{
    public partial class counterformsPage : ContentPage
    {
        Store<int> counter;

        public counterformsPage()
        {
            InitializeComponent();

            counter = new Store<int>((state, action) =>
            {
                if (Equals(action, "+"))
                    return state + 1;
                if (Equals(action, "-"))
                    return state - 1;

                return state;
            }, 0);


            counter.Connect(mapStateToProps: state => new
            {
                count = state
            }, propsChanged: props =>
            {
                CounterValue.Text = props.count.ToString();
            });
        }

        void Up_Clicked(object sender, EventArgs e) => counter.Dispatch("+");
        void Down_Clicked(object sender, EventArgs e) => counter.Dispatch("-");
    }

    public class CounterViewModel
    {
    }
}
