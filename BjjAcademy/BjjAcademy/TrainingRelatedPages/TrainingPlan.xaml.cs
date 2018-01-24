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
    public partial class TrainingPlan : ContentPage
    {
        #region variables

        private Models.TrainingPlan trainingPlan;
        private ObservableCollection<string> exercisesList;

        #endregion

        public TrainingPlan()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Training, Models.TrainingPlan>(this, GlobalMethods.MessagingCenterMessage.TrainingPlanViewed, ReceiveTrainingPlanObject);
        }

        private void ReceiveTrainingPlanObject(Training source, Models.TrainingPlan trainingPlanArg)
        {
            trainingPlan = trainingPlanArg;
            LblTrainingPlanName.Text = trainingPlan.Name;
            exercisesList = trainingPlan.GetTrainingActivities();
            ExercisesList.ItemsSource = exercisesList;
        }

        private void AddExercise_Activated(object sender, EventArgs e)
        {

        }
    }
}