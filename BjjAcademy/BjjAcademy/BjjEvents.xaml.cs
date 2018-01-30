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
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<BjjEvent> EventsList;

        #endregion

        #region Constructor

        public BjjEvents()
        {
            IsStartup = true;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            MessagingCenter.Subscribe<AddUpdateBjjEvent, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.AddedBjjEvent, BjjEventAdded);
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

        private void AddBjjEvent_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddUpdateBjjEvent());
        }

        private async void MiDelete_Clicked(object sender, EventArgs e)
        {
            var index = EventsList.IndexOf((sender as MenuItem).CommandParameter as BjjEvent);

            BjjEvent EventToDelete = EventsList[index];

            await _connection.DeleteAsync(EventToDelete);

            EventsList.RemoveAt(index);
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

        #endregion


    }
}