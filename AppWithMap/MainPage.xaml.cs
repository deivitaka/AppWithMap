using AppWithMap.Common;
using System;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Foundation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace AppWithMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private double lat, lon;

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            getLocation();
        }

        private async void getLocation()
        {
            Geolocator gl = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
            };

            try
            {
                Geoposition gp = await gl.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(10),
                    timeout: TimeSpan.FromSeconds(30));
                //message("Lat: " + gp.Coordinate.Latitude +
                //"\n Lon: " + gp.Coordinate.Longitude, "Coordinates");
                this.lat = gp.Coordinate.Point.Position.Latitude;
                this.lon = gp.Coordinate.Point.Position.Longitude;
                CenterMap(this.lat, this.lon);
                MyMap.ZoomLevel = (int)slider.Value;
            }
            catch (Exception e)
            {
                message(e.Message, "ERROR!");
            }
        }

        private async void message(string body, string title)
        {
            var dlg = new MessageDialog(
                    string.Format(body), title);

            try
            {
                await dlg.ShowAsync();
            }
            catch (Exception) { }
        }

        private void CenterMap(double lat, double lon)
        {
            MyMap.Center =
                new Geopoint(new BasicGeoposition()
                {
                    Latitude = lat,
                    Longitude = lon
                });
        }

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (slider != null)
                MyMap.ZoomLevel = (int)slider.Value;
        }
        
        public void AddPushpin(double lat, double lon, Color c)
        {
            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = lat;
            location.Longitude = lon;
            
            var pin = new Ellipse()
            {
                Fill = new SolidColorBrush(c),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 1,
                Width = 40,
                Height = 40,
            };

            pin.Tapped += pin_Tapped;

            Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(pin, new Geopoint(location));
            MyMap.Children.Add(pin);
        }

        void pin_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            message("This is your location.", "");
        }
        
        private void AppBarToggleButton_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AddPushpin(MyMap.Center.Position.Latitude, MyMap.Center.Position.Longitude, Colors.Blue);
        }

        private void AppBarToggleButton_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MyMap.Children.Clear();
        }
    }
}