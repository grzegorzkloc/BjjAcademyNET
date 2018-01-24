using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.TrainingRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUpdateExercise : ContentPage
    {
        private bool IsAddPage;
        public AddUpdateExercise()
        {
            //Adding Exercise
            IsAddPage = true;
            InitializeComponent();
        }

        public AddUpdateExercise(string exercise)
        {
            //Updating Exercise
            IsAddPage = false;
            InitializeComponent();
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            string exe = EdtrExercise.Text;
            if (IsAddPage)
            {
                try
                {
                    MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.ExerciseAdded, exe);
                }
                catch (Exception oko)
                {
                    var data = oko.Data;
                }
                Navigation.PopAsync();
                //MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.ExerciseAdded, exe);
            }
            else
            {
                Navigation.PopAsync();
                MessagingCenter.Send(this, GlobalMethods.MessagingCenterMessage.ExerciseUpdated, EdtrExercise.Text);
            }
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void EtrExercise_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) AddBtn.IsEnabled = true;
            else AddBtn.IsEnabled = false;
        }


    }
}