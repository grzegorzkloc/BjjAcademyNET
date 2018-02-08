﻿using BjjAcademy.Models;
using BjjAcademy.Persistence;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromotionPage : ContentPage
    {
        //TODO Add sorting by new rank.

        private ObservableCollection<PromotedPerson> Participants;
        private ObservableCollection<Person> AllPeople;
        private Models.BjjEvent _bjjEvent;
        private SQLiteAsyncConnection _connection;

        public PromotionPage()
        {
            _bjjEvent = null;
            InitializeComponent();
        }

        public PromotionPage(Models.BjjEvent Event)
        {
            Participants = new ObservableCollection<PromotedPerson>();
            AllPeople = new ObservableCollection<Person>();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            BindingContext = this;
            Title = Event.EventName;
            _bjjEvent = Event;

            MessagingCenter.Unsubscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToPromotionPage);
            MessagingCenter.Subscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToPromotionPage, PopulateReceivedList);

            MessagingCenter.Unsubscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent);
            MessagingCenter.Subscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent, ReceiveMultiselectedPersons);

            MessagingCenter.Unsubscribe<ChangeBeltForPromotion>(this, GlobalMethods.MessagingCenterMessage.PromotionListEmpty);
            MessagingCenter.Subscribe<ChangeBeltForPromotion>(this, GlobalMethods.MessagingCenterMessage.PromotionListEmpty, CompletePromotion);

            MessagingCenter.Send<PromotionPage>(this, GlobalMethods.MessagingCenterMessage.PromotionPageCreated);

            InitializeComponent();
        }

        private async void CompletePromotion(ChangeBeltForPromotion obj)
        {
            await DisplayAlert("Promocja zakończona", "Wszystkie osoby zostały promowane. Wydarzenie zostanie usunięte.", "OK");
            await Navigation.PopAsync();
            MessagingCenter.Send<PromotionPage, BjjEvent>(this, GlobalMethods.MessagingCenterMessage.DeletePromotionEvent, _bjjEvent);
        }

        private async void ReceiveMultiselectedPersons(MultiselectPersonsPage sender, ObservableCollection<Person> args)
        {
            foreach (var Person in args)
            {
                Belt NewBelt = await GlobalMethods.DbHelper.GetChosenBelt(_connection, Person.BeltId);
                PromotedPerson PersonToBePromoted = new PromotedPerson(Person, NewBelt);
                Participants.Add(PersonToBePromoted);
            }
        }

        private async void PopulateReceivedList(Students sender, ObservableCollection<Person> args)
        {
            AllPeople = args;
            var ParticipantsIdList = new ObservableCollection<int>(Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<int>>(_bjjEvent.ParticipantsBlob));
            var BeltsIdList = new ObservableCollection<byte>(Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<byte>>(_bjjEvent.NewBeltsBlob));
            var BeltsList = new ObservableCollection<Belt>();

            var Belts = await _connection.Table<Belt>().ToListAsync();
            foreach (var beltId in BeltsIdList)
            {
                foreach (var belt in Belts)
                {
                    if (beltId == belt.Id) BeltsList.Add(belt);
                }
            }

            int i = 0;
            foreach (var Id in ParticipantsIdList)
            {
                foreach (var Person in AllPeople)
                {
                    if (Person.Id == Id)
                    {
                        PromotedPerson PersonToBePromoted = new PromotedPerson(Person, BeltsList[i]);
                        Participants.Add(PersonToBePromoted);
                    }
                }
                i++;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ParticipantsList.ItemsSource = Participants;
            NoOfParticipants.Text = Participants.Count.ToString();
        }

        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToPromotionPage);
            //MessagingCenter.Unsubscribe<ChangeBeltForPromotion>(this, GlobalMethods.MessagingCenterMessage.PromotionListEmpty);

            //MessagingCenter.Unsubscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent);

            ObservableCollection<int> ParticipantsID = new ObservableCollection<int>();
            ObservableCollection<int> NewBeltsID = new ObservableCollection<int>();
            foreach (var person in Participants)
            {
                ParticipantsID.Add(person.Person.Id);
                NewBeltsID.Add(person.NewBelt.Id);
            }

            _bjjEvent.ParticipantsBlob = JsonConvert.SerializeObject(ParticipantsID);
            _bjjEvent.NewBeltsBlob = JsonConvert.SerializeObject(NewBeltsID);

            await _connection.UpdateAsync(_bjjEvent);

            base.OnDisappearing();
        }

        private void MiAddPeople_Activated(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            Navigation.PushModalAsync(new MultiselectPersonsPage(AllPeople, Participants));

            ParticipantsList.SelectedItem = null;
        }

        private async Task MiDelete_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Uwaga", "Czy na pewno chcesz usunąć osobę?", "Tak", "Nie"))
            {
                Participants.Remove((sender as MenuItem).CommandParameter as PromotedPerson);
                NoOfParticipants.Text = Participants.Count.ToString();
            }
        }

        private void ParticipantsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ParticipantsList.SelectedItem = null;

            PromotedPerson toSend = e.SelectedItem as PromotedPerson;
            Navigation.PushModalAsync(new ChangeBeltForPromotion(ref toSend, ref Participants));
        }
    }
}