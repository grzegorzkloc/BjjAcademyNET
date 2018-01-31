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
        public MultiselectPersonsPage()
        {
            InitializeComponent();
        }

        public MultiselectPersonsPage(ObservableCollection<Person> Participants)
        {
            InitializeComponent();

        }
    }
}