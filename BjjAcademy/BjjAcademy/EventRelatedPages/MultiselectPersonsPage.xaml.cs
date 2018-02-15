using BjjAcademy.Models;
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
    public partial class MultiselectPersonsPage : ContentPage
    {
        #region Variables

        private ObservableCollection<Person> AllPeople;
        private ObservableCollection<Person> AlreadySelectedPeople;
        private ObservableCollection<Models.SelectedPerson> PeopleToSelectFrom;

        #endregion

        #region Constructors

        public MultiselectPersonsPage()
        {
            InitializeComponent();
        }

        public MultiselectPersonsPage(ObservableCollection<Person> Participants)
        {
            InitializeComponent();
            AllPeople = Participants;
            PeopleToSelectFrom = new ObservableCollection<Models.SelectedPerson>();
        }

        public MultiselectPersonsPage(ObservableCollection<Person> All, ObservableCollection<Person> Participants)
        {
            InitializeComponent();
            AllPeople = All;
            AlreadySelectedPeople = Participants;
            PeopleToSelectFrom = new ObservableCollection<Models.SelectedPerson>();
        }

        public MultiselectPersonsPage(ObservableCollection<Person> All, ObservableCollection<PromotedPerson> Participants)
        {
            InitializeComponent();
            AllPeople = All;
            AlreadySelectedPeople = GetPeopleFromPromotedPeople(Participants);
            PeopleToSelectFrom = new ObservableCollection<Models.SelectedPerson>();
        }


        #endregion

        #region Override

        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (Person person in AllPeople)
            {
                if (!AlreadySelectedPeople.Contains(person))
                {
                    var PersonToSelect = new Models.SelectedPerson(person);
                    PeopleToSelectFrom.Add(PersonToSelect);
                }
            }

            MultiselectList.ItemsSource = PeopleToSelectFrom;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion

        #region Events

        private void MultiselectList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            (e.SelectedItem as Models.SelectedPerson).IsSelected = !(e.SelectedItem as Models.SelectedPerson).IsSelected;
            EnableAddButton();
            MultiselectList.SelectedItem = null;
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            var SelectedPeople = ReturnSelectedPeople();
            MessagingCenter.Send<MultiselectPersonsPage, ObservableCollection<Person>>(this, GlobalMethods.MessagingCenterMessage.MultiselectPersonsSent, SelectedPeople);
            Navigation.PopModalAsync();
        }

        #endregion

        #region Methods

        private void EnableAddButton()
        {
            foreach (var SelectedPerson in PeopleToSelectFrom)
            {
                if (SelectedPerson.IsSelected == true)
                {
                    AddBtn.IsEnabled = true;
                    return;
                }
            }
            AddBtn.IsEnabled = false;
        }

        private ObservableCollection<Person> ReturnSelectedPeople()
        {
            ObservableCollection<Person> SelectedPeople = new ObservableCollection<Person>();

            foreach (var person in PeopleToSelectFrom)
            {
                if (person.IsSelected == true) SelectedPeople.Add(person.Person);
            }

            return SelectedPeople;
        }

        private ObservableCollection<Person> GetPeopleFromPromotedPeople(ObservableCollection<PromotedPerson> Promoted)
        {
            ObservableCollection<Person> People = new ObservableCollection<Person>();

            if (Promoted.Count == 0) return new ObservableCollection<Person>();

            foreach (var person in Promoted)
            {
                People.Add(person.Person);
            }

            return People;
        }

        #endregion
    }
}