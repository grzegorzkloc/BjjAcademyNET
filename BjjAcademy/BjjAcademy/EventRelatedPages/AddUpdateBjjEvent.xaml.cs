using BjjAcademy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUpdateBjjEvent : ContentPage
    {
        #region Variables

        private bool IsAdd;

        private bool IsEventNameOK;

        #endregion
        public AddUpdateBjjEvent()
        {
            InitializeComponent();
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            BjjEventType TempEventType = new Models.BjjEventType();
            if (BjjEventType.SelectedIndex == 0) TempEventType = Models.BjjEventType.AttendanceList;
            else if (BjjEventType.SelectedIndex == 1) TempEventType = Models.BjjEventType.Promotion;

            BjjEvent NewBjjEvent = new BjjEvent
            {
                EventName = BjjEventName.Text,
                EventType = TempEventType
            };

            Navigation.PopAsync();

            MessagingCenter.Send<AddUpdateBjjEvent, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.AddedBjjEvent, NewBjjEvent);
        }

        private void BjjEventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0)
            {
                IsEventNameOK = true;
                CheckFlags();
            }
            else
            {
                IsEventNameOK = false;
                CheckFlags();

            }
        }

        private void CheckFlags()
        {
            if (IsEventNameOK && BjjEventType.SelectedIndex != -1) ActivateButtons(true);
            else ActivateButtons(false);
        }

        private void ActivateButtons(bool v)
        {
            if (v)
            {
                AddBtn.IsEnabled = true;
            }
            else
            {
                AddBtn.IsEnabled = false;
            }
        }

        private void BjjEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFlags();
        }
    }
}