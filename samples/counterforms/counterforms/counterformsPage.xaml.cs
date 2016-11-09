using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
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

    public class CounterModel
    {
        public int Count { get; set; }
    }

    public class CounterViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        CounterModel model;

        public int Count
        {
            get { return model.Count; }
            private set
            {
                if (model.Count != value)
                {
                    model.Count = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }

        public CounterViewModel(CounterModel model)
        {
            this.model = model;

            UpCommand = new RelayCommand(() => Count++);
            DownCommand = new RelayCommand(() => Count--);
        }
    }

    public class ViewModelBase2 : GalaSoft.MvvmLight.ViewModelBase
    {
        protected T Get<T>() { return default(T); }
        protected void Set<T>(T value) { }

        protected Command DispatchCommand<T>(Store<T> store, object action)
        {
            return new Command(() => store.Dispatch(action));
        }
    }

    public class AppState
    {
        public int Counter { get; set; }
    }

    public class CounterViewModel2 : ViewModelBase2
    {
        public int Count { get { return Get<int>(); } set { Set(value); } }

        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }

        public CounterViewModel2(Store<AppState> store)
        {
            store.Subscribe += (state) => Count = state.Counter;

            UpCommand = new RelayCommand(() => store.Dispatch("+"));
            DownCommand = DispatchCommand(store, "-");
        }
    }
}
