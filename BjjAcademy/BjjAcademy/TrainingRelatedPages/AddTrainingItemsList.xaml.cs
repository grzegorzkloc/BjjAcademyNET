using BjjAcademy.Models;
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
    public partial class AddTrainingItemsList : ContentPage
    {
        #region Variables

        #endregion

        #region Constructor

        public AddTrainingItemsList()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void TrainingPlanName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) this.AddBtn.IsEnabled = true;
            else this.AddBtn.IsEnabled = false;
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            TrainingPlan trainingPlan = new TrainingPlan()
            {
                Name = this.TrainingPlanName.Text
            };

            await DisplayAlert("Plan treningowy dodany", "Plan treningowy o nazwie " + this.TrainingPlanName.Text + " dodany", "OK");

            await Navigation.PopModalAsync();

            MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.TrainingPlanAdded, trainingPlan);
        }

        #endregion
    }
}