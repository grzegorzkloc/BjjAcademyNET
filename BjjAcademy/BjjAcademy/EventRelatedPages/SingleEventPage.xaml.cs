using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public SingleEventPage()
        {
            _bjjEvent = null;
            InitializeComponent();
        }

        public SingleEventPage(Models.BjjEvent Event)
        {
            MessagingCenter.Subscribe<Students, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage, PopulateReceivedList);
            _bjjEvent = Event;
            MessagingCenter.Send<SingleEventPage>(this, GlobalMethods.MessagingCenterMessage.SingleEventPageCreated);
            Participants = new ObservableCollection<Person>();
            InitializeComponent();
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
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Students>(this, GlobalMethods.MessagingCenterMessage.SentToSingleEventPage);
            base.OnDisappearing();
            //TODO Serialize object and save to DB
        }

        private void MiAddPeople_Activated(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            Navigation.PushModalAsync(new MultiselectPersonsPage(AllPeople, Participants));

            ParticipantsList.SelectedItem = null;
        }
    }
}