using BjjAcademy.Models;
using BjjAcademy.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeBeltForPromotion : ContentPage
    {
        private PromotedPerson _promotedPerson;
        private SQLiteAsyncConnection _connection;
        private Belt _newBelt;

        public ChangeBeltForPromotion(ref PromotedPerson person)
        {
            _promotedPerson = person;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            _newBelt = new Belt();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            NameSurnameLbl.Text = _promotedPerson.Person.Name + " " + _promotedPerson.Person.Surname;
            PseudoLbl.Text = _promotedPerson.Person.Pseudo;

            Belt belt = new Belt();
            BeltStripesLbl.Text = await GlobalMethods.DbHelper.GetBeltAndStripeInfo(_connection, _promotedPerson.Person.BeltId);

            base.OnAppearing();
        }

        private void pckrBelt_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void pckrStripes_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}