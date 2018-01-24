﻿using BjjAcademy.Models;
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
            MessagingCenter.Subscribe<AddTrainingItemsList, TrainingPlan>(this, GlobalMethods.MessagingCenterMessage.TrainingPlanAdded, TrainingPlanAdded);
            InitializeComponent();
            BindingContext = this;
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

        private async void BjjTrainingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            await Navigation.PushAsync(new TrainingRelatedPages.TrainingPlan());
            MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.TrainingPlanViewed, e.SelectedItem as TrainingPlan);
            BjjTrainingList.SelectedItem = null;
        }

        private async void InitialOperations()
        {
            await _connection.CreateTableAsync<TrainingPlan>();
        }

        private async void TrainingPlanAdded(AddTrainingItemsList source, TrainingPlan trainingPlan)
        {
            startup = false;
            TrainingPlans.Add(trainingPlan);
            await _connection.InsertAsync(trainingPlan);
        }

        private async void miDelete_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var trainingPlan = menuItem.CommandParameter as TrainingPlan;

            if (await DisplayAlert("Uwaga", "Czy jesteś pewny, że chcesz skasować plan treningowy: "
                + trainingPlan.Name + "?", "Kasuj", "Anuluj"))
            {
                await _connection.DeleteAsync(trainingPlan);
                TrainingPlans.Remove(trainingPlan);
            }
        }
    }
}