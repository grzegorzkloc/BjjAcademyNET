using BjjAcademy.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


using Xamarin.Forms;

namespace BjjAcademy
{
    public partial class App : Application
    {
        public int x;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new BjjAcademy.BjjAcademyTabs());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
