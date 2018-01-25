using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjAcademy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.TrainingRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingPlanPage : ContentPage
    {
        #region variables

        private Models.TrainingPlan trainingPlan;

        #endregion

        public TrainingPlanPage(TrainingPlan TrainingPlanObject)
        {
            InitializeComponent();

            trainingPlan = TrainingPlanObject;

            LblTrainingPlanName.Text = trainingPlan.Name;

            ExercisesList.ItemsSource = trainingPlan.GetTrainingActivities();

            MessagingCenter.Subscribe<TrainingRelatedPages.AddUpdateExercise, string>(this,
                GlobalMethods.MessagingCenterMessage.ExerciseAdded, ReceivedNewExercise);

            MessagingCenter.Subscribe<TrainingRelatedPages.AddUpdateExercise, string>(this,
                GlobalMethods.MessagingCenterMessage.ExerciseUpdated, ReceivedUpdatedExercise);
        }

        public TrainingPlanPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<TrainingRelatedPages.AddUpdateExercise, string>(this,
                GlobalMethods.MessagingCenterMessage.ExerciseAdded, ReceivedNewExercise);

            MessagingCenter.Subscribe<TrainingRelatedPages.AddUpdateExercise, string>(this,
                GlobalMethods.MessagingCenterMessage.ExerciseUpdated, ReceivedUpdatedExercise);
        }

        private void ReceivedUpdatedExercise(AddUpdateExercise source, string exercise)
        {
            //trainingPlan.AddTrainingActivity(exercise);
        }

        private void ReceivedNewExercise(AddUpdateExercise source, string exercise)
        {
            trainingPlan.AddTrainingActivity(exercise);
        }

        private void AddExercise_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddUpdateExercise());
        }

    }


}
