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
        private bool AddTrainingOneClick;
        #endregion

        public Training()
        {
            startup = true;
            AddTrainingOneClick = true;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            MessagingCenter.Subscribe<AddUpdateTrainingItems, TrainingPlan>(this, GlobalMethods.MessagingCenterMessage.TrainingPlanAdded, TrainingPlanAdded);
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            if (startup) InitialOperations();
            base.OnAppearing();
        }

        private async Task AddTrainingUnit_Activated(object sender, EventArgs e)
        {
            //Prevent AddTraining double click
            if (AddTrainingOneClick) AddTrainingOneClick = false;
            else return;

            await Navigation.PushModalAsync(new AddUpdateTrainingItems());

            AddTrainingOneClick = true;
        }

        private async void BjjTrainingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            TrainingPlan PlanToBeViewed = e.SelectedItem as TrainingPlan;

            await Navigation.PushAsync(new TrainingRelatedPages.TrainingPlanPage(ref PlanToBeViewed));
            BjjTrainingList.SelectedItem = null;
        }

        private async void InitialOperations()
        {
            startup = false;
            await _connection.CreateTableAsync<TrainingPlan>();
            var Trainings = await _connection.Table<TrainingPlan>().ToListAsync();
            TrainingPlans = new ObservableCollection<TrainingPlan>(Trainings);
            this.BjjTrainingList.ItemsSource = TrainingPlans;

        }

        private async void TrainingPlanAdded(AddUpdateTrainingItems source, TrainingPlan trainingPlan)
        {
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

        private void MiEdit_Clicked(object sender, EventArgs e)
        {
            var ChosenTrainingPlan = sender as MenuItem;
            var IndexOfTrainingPlanNameToBeEdited = TrainingPlans.IndexOf(ChosenTrainingPlan.CommandParameter as TrainingPlan);
            var TrainingPlanNameToBeEdited = TrainingPlans.ElementAt<TrainingPlan>(IndexOfTrainingPlanNameToBeEdited);
            Navigation.PushModalAsync(new AddUpdateTrainingItems(ref TrainingPlanNameToBeEdited));
        }
    }
}