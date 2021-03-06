﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjAcademy.Models;
using BjjAcademy.Persistence;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.TrainingRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingPlanPage : ContentPage
    {
        #region variables

        private Models.TrainingPlan trainingPlan;
        private ObservableCollection<string> TrainingActivities;
        private SQLiteAsyncConnection _connection;
        private bool IsExerciseAdd;
        private int IndexToEdit;

        #endregion

        #region Constructor

        public TrainingPlanPage(ref TrainingPlan TrainingPlanObject)
        {
            InitializeComponent();

            IsExerciseAdd = true;

            trainingPlan = TrainingPlanObject;

            LblTrainingPlanName.Text = trainingPlan.Name;

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            TrainingActivities = new ObservableCollection<string>();

            DeserializeTrainingPlan();

            ExercisesList.ItemsSource = TrainingActivities;
        }

        public TrainingPlanPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Override

        protected override bool OnBackButtonPressed()
        {
            if (SlListview.IsVisible)
            {
                SerializeTrainingPlan();
                return base.OnBackButtonPressed();
            }
            else
            {
                SlListview.IsVisible = true;

                SlAddExercise.IsVisible = false;
                EdtrExercise.Text = "";
                return true;
            }
        }

        protected override void OnDisappearing()
        {
            SerializeTrainingPlan();
            base.OnDisappearing();
        }

        #endregion

        #region Events

        private void AddExercise_Activated(object sender, EventArgs e)
        {
            AddBtn.IsEnabled = true;
            CancelBtn.IsEnabled = true;

            IsExerciseAdd = true;
            AddBtn.Text = "Dodaj";
            SlAddExercise.IsVisible = true;
            SlListview.IsVisible = false;
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            CancelBtn.IsEnabled = false;
            SlAddExercise.IsVisible = false;
            SlListview.IsVisible = true;
            EdtrExercise.Text = "";
        }

        private async Task AddBtn_Clicked(object sender, EventArgs e)
        {
            AddBtn.IsEnabled = false;
            await AddUpdateExercise(IsExerciseAdd);
        }

        private void EdtrExercise_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) AddBtn.IsEnabled = true;
            else AddBtn.IsEnabled = false;
        }

        private void ExercisesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ExercisesList.SelectedItem = null;
        }

        private async void MiDelete_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Uwaga", "Czy na pewno chcesz usunąć ćwiczenie?", "Tak", "Nie"))
            {
                var ChosenMenuItem = (MenuItem)sender;
                var index = TrainingActivities.IndexOf(ChosenMenuItem.CommandParameter);
                TrainingActivities.RemoveAt(index);
            }
        }

        private void MiEdit_Clicked(object sender, EventArgs e)
        {
            IsExerciseAdd = false;
            AddBtn.Text = "Zmień";

            var ChosenMenuItem = (MenuItem)sender;
            var index = TrainingActivities.IndexOf(ChosenMenuItem.CommandParameter);

            SlAddExercise.IsVisible = true;
            EdtrExercise.Text = TrainingActivities[index];

            IndexToEdit = index;
        }

        private void AddBlankLineBelow_Clicked(object sender, EventArgs e)
        {
            var ChosenMenuItem = (MenuItem)sender;
            var index = TrainingActivities.IndexOf(ChosenMenuItem.CommandParameter);
            TrainingActivities.Insert(index + 1, "");
        }

        #endregion

        #region Methods

        private void DeserializeTrainingPlan()
        {
            TrainingActivities = JsonConvert.DeserializeObject<ObservableCollection<string>>(trainingPlan.TrainingActivitiesBlob);
        }

        private void SerializeTrainingPlan()
        {
            trainingPlan.TrainingActivitiesBlob = JsonConvert.SerializeObject(TrainingActivities);
            _connection.UpdateAsync(trainingPlan);
        }

        private async Task AddUpdateExercise(bool IsAdd)
        {
            if (IsAdd)
            {
                TrainingActivities.Add(EdtrExercise.Text);
                await DisplayAlert("Dodano", "Dodano nowe ćwiczenie", "OK");
                EdtrExercise.Text = "";
                SlAddExercise.IsVisible = false;
                SlListview.IsVisible = true;
            }
            else
            {
                TrainingActivities[IndexToEdit] = EdtrExercise.Text;
                await DisplayAlert("Zaktualizowano", "Ćwiczenie zaktualizowane", "OK");
                EdtrExercise.Text = "";
                SlAddExercise.IsVisible = false;
                SlListview.IsVisible = true;
            }
        }

        #endregion


    }


}
