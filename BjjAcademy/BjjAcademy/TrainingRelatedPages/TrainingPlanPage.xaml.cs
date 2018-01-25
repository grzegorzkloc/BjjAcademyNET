using System;
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

        #endregion

        public TrainingPlanPage(ref TrainingPlan TrainingPlanObject)
        {
            InitializeComponent();

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

        private void AddExercise_Activated(object sender, EventArgs e)
        {
            SlAddExercise.IsVisible = true;
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            SlAddExercise.IsVisible = false;
            EdtrExercise.Text = "";
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            TrainingActivities.Add(EdtrExercise.Text);
            await DisplayAlert("Dodano", "Dodano nowe ćwiczenie", "OK");
            EdtrExercise.Text = "";
            SlAddExercise.IsVisible = false;
        }

        private void EdtrExercise_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) AddBtn.IsEnabled = true;
            else AddBtn.IsEnabled = false;
        }

        protected override void OnDisappearing()
        {
            SerializeTrainingPlan();
            base.OnDisappearing();
        }

        private void DeserializeTrainingPlan()
        {
            TrainingActivities = JsonConvert.DeserializeObject<ObservableCollection<string>>(trainingPlan.TrainingActivitiesBlob);
        }

        private void SerializeTrainingPlan()
        {
            trainingPlan.TrainingActivitiesBlob = JsonConvert.SerializeObject(TrainingActivities);
            _connection.UpdateAsync(trainingPlan);
        }

        private void ExercisesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ExercisesList.SelectedItem = null;
        }

        private void MiDelete_Clicked(object sender, EventArgs e)
        {
            var ChosenMenuItem = (MenuItem)sender;
            var index = TrainingActivities.IndexOf(ChosenMenuItem.CommandParameter);
            TrainingActivities.RemoveAt(index);
        }
    }


}
