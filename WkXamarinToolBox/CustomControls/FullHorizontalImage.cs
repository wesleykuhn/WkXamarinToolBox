using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WkXamarinToolBox.CustomControls
{
    public class FullHorizontalImage : Image
    {
        private const float SizeRatio = 1.5f;
        private double _screenWidth;

        public FullHorizontalImage()
        {
            _screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

            HorizontalOptions = LayoutOptions.StartAndExpand;
            PropertyChanged += new PropertyChangedEventHandler(OnThisPropertyChanged);
        }

        private void OnThisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Height) && Height > 0)
            {
                PropertyChanged -= new PropertyChangedEventHandler(OnThisPropertyChanged);

                WidthRequest = _screenWidth;
                HeightRequest = _screenWidth / SizeRatio;

                PropertyChanged += new PropertyChangedEventHandler(OnThisPropertyChanged);
            }
        }
    }
}
