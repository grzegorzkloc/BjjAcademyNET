using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjAcademy.Persistence;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleEventPage : ContentPage
    {
        private ObservableCollection<Person> Participants;
        private ObservableCollection<Person> AllPeople;
        private Models.BjjEvent _bjjEvent;
        private SQLiteAsyncConnection _connection;

        public SingleEventPage()
        {
            _bjjEvent = null;
            InitializeComponent();
        }

        public SingleEventPage(Models.BjjEvent Event)
        {
            Participants = new ObservableCollection<Person>();
            BindingContext = this;
            Title = Event.EventName;

            MessagingCenter.Unsubscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage);
            MessagingCenter.Subscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage, PopulateReceivedList);

            MessagingCenter.Unsubscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent);
            MessagingCenter.Subscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent, ReceiveMultiselectedPersons);
            _bjjEvent = Event;
            MessagingCenter.Send<SingleEventPage>(this, GlobalMethods.MessagingCenterMessage.SingleEventPageCreated);
            InitializeComponent();
        }

        private void ReceiveMultiselectedPersons(MultiselectPersonsPage sender, ObservableCollection<Person> args)
        {
            foreach (var Person in args)
            {
                Participants.Add(Person);
            }
        }

        private void PopulateReceivedList(Students sender, ObservableCollection<Person> args)
        {
            //TODO Check if List Contains somenthing
            AllPeople = args;
            var ParticipantsIdList = new ObservableCollection<int>(Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<int>>(_bjjEvent.ParticipantsBlob));

            foreach (var Id in ParticipantsIdList)
            {
                foreach (var Person in AllPeople)
                {
                    if (Person.Id == Id) Participants.Add(Person);
                }
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
            MessagingCenter.Unsubscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage);
            //MessagingCenter.Unsubscribe<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent);

            ObservableCollection<int> ParticipantsID = new ObservableCollection<int>();

            foreach (var person in Participants)
            {
                ParticipantsID.Add(person.Id);
            }

            _bjjEvent.ParticipantsBlob = JsonConvert.SerializeObject(ParticipantsID);
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

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
                Participants.Remove((sender as MenuItem).CommandParameter as Person);
                NoOfParticipants.Text = Participants.Count.ToString();
            }
        }

        private void ParticipantsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ParticipantsList.SelectedItem = null;
        }
    }
}