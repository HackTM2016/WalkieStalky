﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Position = Xamarin.Forms.Maps.Position;

namespace WalkieStalky.Views
{
    public partial class MapPage : ContentPage
    {
        private double _currentLatitude;
        private double _currentLongitude;
        public bool Initialized { get; set; }

        public MapPage()
        {
            InitializeComponent();
            Initialized = false;
            

            //GestureFrame.SwipeDown += (s, e) =>
            //{
            //    var initialBearingRadians = 0;
            //    MoveLat(initialBearingRadians);
            //};

            //GestureFrame.SwipeTop += (s, e) =>
            //{
            //    MoveLat(180);
            //};

            //GestureFrame.SwipeLeft += (s, e) =>
            //{
            //    MoveLong(90);
            //};

            //GestureFrame.SwipeRight += (s, e) =>
            //{
            //    MoveLong(270);
            //};
            //GestureFrame.Tap += (s, e) =>
            //{
             
            //};
            

        }


        private void MoveLong(int initialBearingRadians)
        {
            var newLocation =
                FindPointAtDistanceFrom(
                    new GeoLocation {Latitude = _currentLatitude, Longitude = _currentLongitude},
                    initialBearingRadians, 2);
           
           // _currentLatitude = newLocation.Latitude;
            _currentLongitude = newLocation.Longitude;
            var mapSpan = MapSpan.FromCenterAndRadius(
               new Position(_currentLatitude, _currentLongitude), Distance.FromKilometers(1));
            TopicsMap.MoveToRegion(mapSpan);
        }

        private void MoveLat(int initialBearingRadians)
        {
            var newLocation =
                FindPointAtDistanceFrom(
                    new GeoLocation { Latitude = _currentLatitude, Longitude = _currentLongitude },
                    initialBearingRadians,2);
            
            _currentLatitude = newLocation.Latitude;
            //  _currentLongitude = newLocation.Longitude;
            var mapSpan = MapSpan.FromCenterAndRadius(
        new Position(_currentLatitude, _currentLongitude), Distance.FromKilometers(1));
            TopicsMap.MoveToRegion(mapSpan);
        }

        public void Initialize()
        {
            Initialized = true;
            GetCurrentLocation();
            TopicsMap.IsShowingUser = true;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        async void GetCurrentLocation()
        {
            var location =await CrossGeolocator.Current.GetPositionAsync();
            var mapSpan = MapSpan.FromCenterAndRadius(
               new Position(location.Latitude, location.Longitude), Distance.FromKilometers(1));
            _currentLatitude = location.Latitude;
            _currentLongitude = location.Longitude;
            TopicsMap.MoveToRegion(mapSpan);
            var item = new Pin
            {
                Type = PinType.Generic,
                Position = new Position(_currentLatitude,_currentLongitude),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA"
            };
            TopicsMap.CustomPins.Add(new CustomPin {Pin = item,ImageSource = "tomato.png" });
            TopicsMap.Pins.Add(item);
            


        }

     

        public static GeoLocation FindPointAtDistanceFrom(GeoLocation startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Latitude);
            var startLonRad = DegreesToRadians(startPoint.Longitude);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad
                + Math.Atan2(
                    Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                    distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GeoLocation
            {
                Latitude = RadiansToDegrees(endLatRads),
                Longitude = RadiansToDegrees(endLonRads)
            };
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }

        private void OnTapped(object sender, EventArgs e)
        {
            
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            GetCurrentLocation();
        }

        private async void OnModalClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MatchPage());
        }
    }

  

    public struct GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

   
}
