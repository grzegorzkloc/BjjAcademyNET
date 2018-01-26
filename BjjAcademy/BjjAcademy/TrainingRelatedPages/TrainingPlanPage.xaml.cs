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
            SerializeTrainingPlan();
            return base.OnBackButtonPressed();
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
            IsExerciseAdd = true;
            AddBtn.Text = "Dodaj";
            SlAddExercise.IsVisible = true;
        }



        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            SlAddExercise.IsVisible = false;
            EdtrExercise.Text = "";
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            AddUpdateExercise(IsExerciseAdd);
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

        private void MiDelete_Clicked(object sender, EventArgs e)
        {
            var ChosenMenuItem = (MenuItem)sender;
            var index = TrainingActivities.IndexOf(ChosenMenuItem.CommandParameter);
            TrainingActivities.RemoveAt(index);
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

        private async void AddUpdateExercise(bool IsAdd)
        {
            if (IsAdd)
            {
                TrainingActivities.Add(EdtrExercise.Text);
                await DisplayAlert("Dodano", "Dodano nowe ćwiczenie", "OK");
                EdtrExercise.Text = "";
                SlAddExercise.IsVisible = false;
            }
            else
            {
                TrainingActivities[IndexToEdit] = EdtrExercise.Text;
                await DisplayAlert("Zaktualizowano", "Ćwiczenie zaktualizowane", "OK");
                EdtrExercise.Text = "";
                SlAddExercise.IsVisible = false;
            }
        }

        #endregion






    }


}
