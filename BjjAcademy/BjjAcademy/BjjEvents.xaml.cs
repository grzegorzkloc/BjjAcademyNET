using BjjAcademy.Models;
using BjjAcademy.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BjjEvents : ContentPage
    {
        #region Variables

        private SQLiteAsyncConnection _connection;
        private ObservableCollection<BjjEvent> EventsList;

        #endregion

        #region Constructor

        public BjjEvents()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            EventsList = new ObservableCollection<BjjEvent>();

            InitializeComponent();
        }

        #endregion

        #region Override

        protected override void OnAppearing()
        {
            EventsList = new ObservableCollection<BjjEvent>();
            base.OnAppearing();
        }

        #endregion

        #region Events

        #endregion

        private void AddBjjEvent_Activated(object sender, EventArgs e)
        {

        }
    }
}