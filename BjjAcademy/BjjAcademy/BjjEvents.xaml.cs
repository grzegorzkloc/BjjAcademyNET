﻿using BjjAcademy.EventRelatedPages;
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
    public partial class BjjEvents : ContentPage
    {
        #region Variables

        private bool IsStartup;
        private bool AddEventOneClick;
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<BjjEvent> EventsList;

        #endregion

        #region Constructor

        public BjjEvents()
        {
            IsStartup = true;
            AddEventOneClick = true;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            MessagingCenter.Unsubscribe<AddUpdateBjjEvent, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.AddedBjjEvent);
            MessagingCenter.Subscribe<AddUpdateBjjEvent, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.AddedBjjEvent, BjjEventAdded);

            MessagingCenter.Unsubscribe<PromotionPage, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.DeletePromotionEvent);
            MessagingCenter.Subscribe<PromotionPage, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.DeletePromotionEvent, DeleteCompletedPromotion);

            BindingContext = this;
            InitializeComponent();
        }

        #endregion

        #region Override

        protected override async void OnAppearing()
        {
            if (IsStartup) await InitialOperarions();
            BjjEventList.ItemsSource = EventsList;
            base.OnAppearing();
        }

        #endregion

        #region Events

        private async Task AddBjjEvent_Activated(object sender, EventArgs e)
        {
            //Prevent Add Event double click
            if (AddEventOneClick) AddEventOneClick = false;
            else return;

            await Navigation.PushModalAsync(new AddUpdateBjjEvent());

            AddEventOneClick = true;
        }

        private async Task MiDelete_Clicked(object sender, EventArgs e)
        {
            var index = EventsList.IndexOf((sender as MenuItem).CommandParameter as BjjEvent);

            BjjEvent EventToDelete = EventsList[index];

            if (await DisplayAlert("Uwaga", "Czy chcesz skasować wydarzenie o nazwie" + EventToDelete.EventName + "?", "Tak", "Nie"))
            {

                await _connection.DeleteAsync(EventToDelete);

                EventsList.RemoveAt(index);
            }
        }

        private async Task MiEdit_Clicked(object sender, EventArgs e)
        {
            var EventToEditIndex = EventsList.IndexOf((sender as MenuItem).CommandParameter as BjjEvent);
            var EventToEdit = EventsList.ElementAt<BjjEvent>(EventToEditIndex);
            await Navigation.PushModalAsync(new AddUpdateBjjEvent(ref EventToEdit));
        }

        private async Task BjjEventList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            if ((e.SelectedItem as Models.BjjEvent).EventType == BjjEventType.AttendanceList)
                await Navigation.PushAsync(new SingleEventPage(e.SelectedItem as Models.BjjEvent));
            else if ((e.SelectedItem as Models.BjjEvent).EventType == BjjEventType.Promotion)
                await Navigation.PushAsync(new PromotionPage(e.SelectedItem as Models.BjjEvent));
            BjjEventList.SelectedItem = null;
        }

        #endregion

        #region Methods

        private async Task InitialOperarions()
        {
            await _connection.CreateTableAsync<BjjEvent>();
            var EventsFromDatabase = await _connection.Table<BjjEvent>().ToListAsync();
            EventsList = new ObservableCollection<BjjEvent>(EventsFromDatabase);

            IsStartup = false;
        }

        #endregion

        #region MessagingCenterMethods

        private async void BjjEventAdded(AddUpdateBjjEvent sender, BjjEvent args)
        {
            EventsList.Add(args as BjjEvent);
            await _connection.InsertAsync(args as BjjEvent);
        }

        private async void DeleteCompletedPromotion(PromotionPage arg1, BjjEvent arg2)
        {
            await _connection.DeleteAsync(arg2);
            EventsList.Remove(arg2);
        }

        #endregion


    }
}