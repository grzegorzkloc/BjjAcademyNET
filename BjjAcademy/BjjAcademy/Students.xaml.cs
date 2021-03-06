﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using SQLitePCL;
using BjjAcademy.Persistence;
using BjjAcademy;
using System.Collections.ObjectModel;
using BjjAcademy.GlobalMethods;
using PCLStorage;
using BjjAcademy.EventRelatedPages;

namespace BjjAcademy
{
    public partial class Students : ContentPage
    {
        #region Variables

        private bool startup;
        private bool AddUpdatePersonOneClick;
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Person> PersonsList;

        #endregion

        #region Constructor

        public Students()
        {
            startup = true;
            AddUpdatePersonOneClick = true;

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            PersonsList = new ObservableCollection<Person>();
            BindingContext = this;
            InitializeComponent();
        }

        #endregion

        #region Overriding

        protected override async void OnAppearing()
        {
            /* Initial operations consist of creating tables and filling the 
             * belt table on app's first run */
            if (startup)
            {
                startup = false;
                await InitialOperations();

                var Persons = await _connection.Table<Person>().ToListAsync();
                foreach (Person person in Persons)
                {
                    if (!String.IsNullOrEmpty(person.Photo))
                    {
                        var file = await PCLStorage.FileSystem.Current.GetFileFromPathAsync(person.Photo);
                        if (file == null)
                        {
                            person.Photo = null;
                            await _connection.UpdateAsync(person);
                        }
                    }
                }
                PersonsList = new ObservableCollection<Person>(Persons);
                StudentList.ItemsSource = PersonsList;

                MessagingCenter.Unsubscribe<AddUpdatePersonPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.PersonUpdated);
                MessagingCenter.Subscribe<AddUpdatePersonPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.PersonUpdated, PersonUpdated);

                MessagingCenter.Unsubscribe<SingleEventPage>(this, GlobalMethods.MessagingCenterMessage.SingleEventPageCreated);
                MessagingCenter.Subscribe<SingleEventPage>(this, GlobalMethods.MessagingCenterMessage.SingleEventPageCreated, SendPersonsListToSingleEventPage);

                MessagingCenter.Unsubscribe<PromotionPage>(this, GlobalMethods.MessagingCenterMessage.PromotionPageCreated);
                MessagingCenter.Subscribe<PromotionPage>(this, GlobalMethods.MessagingCenterMessage.PromotionPageCreated, SendPersonsListToPromotionPage);
            }
            UpdateListview();
            base.OnAppearing();
        }

        #endregion

        #region Events

        private async Task AddPerson_Activated(object sender, EventArgs e)
        {
            //Prevent double click
            if (AddUpdatePersonOneClick) AddUpdatePersonOneClick = false;
            else return;

            if (!String.IsNullOrWhiteSpace(searchbar.Text)) searchbar.Text = "";

            await Navigation.PushModalAsync(new AddUpdatePersonPage(_connection, ref PersonsList));
            AddUpdatePersonOneClick = true;
        }

        private async void StudentList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var student = e.SelectedItem as Person;
            await Navigation.PushAsync(new PersonDetails(ref student, _connection, ref PersonsList));
            StudentList.SelectedItem = null;

            if (!String.IsNullOrWhiteSpace(searchbar.Text)) searchbar.Text = "";
        }

        private void StudentList_Refreshing(object sender, EventArgs e)
        {
            UpdateListview();
            StudentList.EndRefresh();
        }

        private void Searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            StudentList.BeginRefresh();
            if (String.IsNullOrWhiteSpace(e.NewTextValue)) StudentList.ItemsSource = PersonsList;
            else StudentList.ItemsSource = PersonsList.Where<Person>(i => i.Name.ToLower().StartsWith(e.NewTextValue.ToLower())
            || i.Surname.ToLower().StartsWith(e.NewTextValue.ToLower()) || i.Pseudo != null && (i.Pseudo.ToLower().StartsWith(e.NewTextValue.ToLower())));

            StudentList.EndRefresh();
        }

        private async void MenuItemDelete_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var person = menuItem.CommandParameter as Person;

            if (await DisplayAlert("Uwaga", "Czy jesteś pewny, że chcesz skasować osobę: "
                + person.Name + " "
                + person.Surname, "Kasuj", "Anuluj"))
            {
                if (!String.IsNullOrEmpty(person.Photo))
                {
                    var file = await FileSystem.Current.GetFileFromPathAsync(person.Photo);
                    if (file != null) await file.DeleteAsync();
                }

                await _connection.DeleteAsync(person);
                PersonsList.Remove(person);
                StudentList.ItemsSource = PersonsList;
                if (!String.IsNullOrWhiteSpace(searchbar.Text)) searchbar.Text = "";
                StudentList.EndRefresh();
            }
        }

        #endregion

        #region Methods

        public async Task InitialOperations()
        {
            await _connection.CreateTableAsync<Belt>();
            await _connection.CreateTableAsync<Person>();

            int BeltCount = await _connection.Table<Belt>().CountAsync();

            if (BeltCount == 0)
            {
                for (int i = 0; i <= 4; i++)
                {
                    for (int j = 0; j <= 4; j++)
                    {
                        Belt belt = new Belt((BeltColour)i, (byte)j);
                        await _connection.InsertAsync(belt);
                    }

                }
            }
        }

        private void PersonUpdated(AddUpdatePersonPage source, ObservableCollection<Person> list)
        {
            //TODO check if this can be deleted
            PersonsList = list;
            UpdateListview();
        }

        private void UpdateListview()
        {
            //StudentList.ItemsSource = null;
            //StudentList.ItemsSource = PersonsList;
        }

        private void SendPersonsListToSingleEventPage(SingleEventPage source)
        {
            MessagingCenter.Send<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage, PersonsList);
        }

        private void SendPersonsListToPromotionPage(PromotionPage obj)
        {
            MessagingCenter.Send<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToPromotionPage, PersonsList);
        }

        #endregion
    }
}
