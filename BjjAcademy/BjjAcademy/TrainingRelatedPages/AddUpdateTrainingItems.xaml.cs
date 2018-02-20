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
    public partial class AddUpdateTrainingItems : ContentPage
    {
        #region Variables

        private TrainingPlan _trainingPlanNameToBeEdited;
        private bool IsAddPage;
        private SQLiteAsyncConnection _connection;

        #endregion

        #region Constructor

        public AddUpdateTrainingItems()
        {
            IsAddPage = true;
            InitializeComponent();
        }

        public AddUpdateTrainingItems(ref TrainingPlan TrainingPlanNameToBeEdited)
        {
            IsAddPage = false;
            InitializeComponent();
            _trainingPlanNameToBeEdited = TrainingPlanNameToBeEdited;
            TrainingPlanName.Text = _trainingPlanNameToBeEdited.Name;
            AddBtn.Text = "Zmień";
            lblPageTitle.Text = "Edytuj nazwę planu treningowego";
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        #endregion

        #region Events

        private void TrainingPlanName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) this.AddBtn.IsEnabled = true;
            else this.AddBtn.IsEnabled = false;
        }

        private async Task CancelBtn_Clicked(object sender, EventArgs e)
        {
            CancelBtn.IsEnabled = false;
            await Navigation.PopModalAsync();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            AddBtn.IsEnabled = false;
            if (IsAddPage) await AddTrainingPlan();
            else await ChangeTrainingPlanName();
        }

        #endregion

        #region Methods

        private async Task AddTrainingPlan()
        {
            TrainingPlan trainingPlan = new TrainingPlan()
            {
                Name = this.TrainingPlanName.Text
            };

            await DisplayAlert("Plan treningowy dodany", "Plan treningowy o nazwie " + this.TrainingPlanName.Text + " dodany", "OK");

            await Navigation.PopModalAsync();

            MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.TrainingPlanAdded, trainingPlan);
        }

        private async Task ChangeTrainingPlanName()
        {
            _trainingPlanNameToBeEdited.Name = this.TrainingPlanName.Text;

            await DisplayAlert("Plan treningowy zmieniony", message: "Nazwa planu treningowego została zmieniona", cancel: "OK");

            await _connection.UpdateAsync(_trainingPlanNameToBeEdited);

            await Navigation.PopModalAsync();
        }

        #endregion
    }
}