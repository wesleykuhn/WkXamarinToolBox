using System.Windows.Input;
using Xamarin.Forms;

namespace WkXamarinToolBox.CustomControls
{
    public class CustomListView : ListView
    {
        public CustomListView()
        {
            ItemTapped += OnItemTapped;
        }

        public static BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(CustomListView), null);
        public ICommand ItemTappedCommand
        {
            get => (ICommand)GetValue(ItemTappedCommandProperty);
            set => SetValue(ItemTappedCommandProperty, value);
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                ItemTappedCommand?.Execute(e.Item);

                SelectedItem = null;
            }
        }
    }
}
