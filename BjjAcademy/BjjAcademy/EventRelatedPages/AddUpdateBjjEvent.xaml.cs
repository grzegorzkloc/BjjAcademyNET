﻿using BjjAcademy.Models;
using BjjAcademy.Persistence;
using SQLite;
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
        private SQLiteAsyncConnection _connection;
        private bool IsEventNameOK;

        BjjEvent EventToBeEdited;

        #endregion
        public AddUpdateBjjEvent()
        {
            IsAdd = true;
            InitializeComponent();
        }

        public AddUpdateBjjEvent(ref BjjEvent EventToEdit)
        {
            IsAdd = false;
            BjjEventType.IsEnabled = false;
            InitializeComponent();
            EventToBeEdited = EventToEdit;
            SetFields(EventToBeEdited);
        }

        private void SetFields(BjjEvent eventToBeEdited)
        {
            AddBtn.Text = "Zmień";

            BjjEventName.Text = eventToBeEdited.EventName;
            if (eventToBeEdited.EventType == Models.BjjEventType.AttendanceList) BjjEventType.SelectedIndex = 0;
            else if (eventToBeEdited.EventType == Models.BjjEventType.Promotion) BjjEventType.SelectedIndex = 1;
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            if (IsAdd)
            {
                BjjEventType TempEventType = new Models.BjjEventType();
                if (BjjEventType.SelectedIndex == 0) TempEventType = Models.BjjEventType.AttendanceList;
                else if (BjjEventType.SelectedIndex == 1) TempEventType = Models.BjjEventType.Promotion;

                BjjEvent NewBjjEvent = new BjjEvent
                {
                    EventName = BjjEventName.Text,
                    EventType = TempEventType
                };

                await Navigation.PopModalAsync();

                MessagingCenter.Send<AddUpdateBjjEvent, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.AddedBjjEvent, NewBjjEvent);
            }
            else
            {
                EventToBeEdited.EventName = BjjEventName.Text;
                if (BjjEventType.SelectedIndex == 0) EventToBeEdited.EventType = Models.BjjEventType.AttendanceList;
                else if (BjjEventType.SelectedIndex == 1) EventToBeEdited.EventType = Models.BjjEventType.Promotion;
                _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                await _connection.UpdateAsync(EventToBeEdited);
                await Navigation.PopModalAsync();
            }
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

//TODO Add Display Alerts on Add and Edit

//TODO Clean up this mess (this file)