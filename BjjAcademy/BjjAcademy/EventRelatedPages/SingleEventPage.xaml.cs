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
    public partial class SingleEventPage : ContentPage
    {
        private ObservableCollection<Person> Participants;

        public SingleEventPage()
        {
            InitializeComponent();
        }

        public SingleEventPage(Models.BjjEvent Event)
        {
            Participants = new ObservableCollection<Person>();
            ParticipantsList.ItemsSource = Participants;
            InitializeComponent();
        }

        private void MiAddPeople_Clicked(object sender, EventArgs e)
        {

        }
    }
}