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
        private ObservableCollection<Person> AllPeople;
        private ObservableCollection<Models.SelectedPerson> PeopleToSelectFrom;

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
            PeopleToSelectFrom = new ObservableCollection<Models.SelectedPerson>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (Person person in AllPeople)
            {
                var PersonToSelect = new Models.SelectedPerson(person);
                PeopleToSelectFrom.Add(PersonToSelect);
            }

            MultiselectList.ItemsSource = PeopleToSelectFrom;
        }

        private void MultiselectList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            (e.SelectedItem as Models.SelectedPerson).IsSelected = !(e.SelectedItem as Models.SelectedPerson).IsSelected;
            MultiselectList.SelectedItem = null;
        }

    }
}