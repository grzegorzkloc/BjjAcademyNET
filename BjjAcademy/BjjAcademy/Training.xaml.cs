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
    public partial class Training : ContentPage
    {
        #region Variables

        private ObservableCollection<TrainingPlan> TrainingPlans;
        private SQLiteAsyncConnection _connection;
        private bool startup;

        #endregion

        public Training()
        {
            startup = true;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (startup) InitialOperations();
            var Trainings = await _connection.Table<TrainingPlan>().ToListAsync();
            TrainingPlans = new ObservableCollection<TrainingPlan>(Trainings);
            this.BjjTrainingList.ItemsSource = TrainingPlans;
            base.OnAppearing();
        }

        private void AddTrainingUnit_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddTrainingItemsList());
        }

        private void BjjTrainingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private async void InitialOperations()
        {
            await _connection.CreateTableAsync<TrainingPlan>();
        }
    }
}