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

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeBeltForPromotion : ContentPage
    {
        #region Variables

        private PromotedPerson _promotedPerson;
        private SQLiteAsyncConnection _connection;
        private Belt _newBelt;
        private ObservableCollection<PromotedPerson> _list;

        #endregion

        #region Constructor

        public ChangeBeltForPromotion(ref PromotedPerson person, ref ObservableCollection<PromotedPerson> PromotedPersonsList)
        {
            _promotedPerson = person;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            _newBelt = new Belt();
            _list = PromotedPersonsList;

            InitializeComponent();
        }

        #endregion

        #region Override

        protected override async void OnAppearing()
        {
            NameSurnameLbl.Text = _promotedPerson.Person.Name + " " + _promotedPerson.Person.Surname;
            PseudoLbl.Text = _promotedPerson.Person.Pseudo;

            if (IsImageVisible()) this.PersonPhoto.Source = _promotedPerson.Person.Photo;

            await SetOldBeltInfo();

            SetPickers();

            base.OnAppearing();
        }

        #endregion

        #region Events

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void ApproveBtn_Clicked(object sender, EventArgs e)
        {
            SaveState();
            Navigation.PopModalAsync();
        }

        private async Task PromoteBtn_Clicked(object sender, EventArgs e)
        {
            //TODO Check if list contains people and if not, delete promotion object

            if ((byte)await GetBeltIdFromPickers() <= _promotedPerson.Person.BeltId)
            {
                await DisplayAlert("Błąd", "Nie możesz promować osoby na niższy stopień", "OK");
            }
            else
            {
                _promotedPerson.Person.BeltId = (byte)await GetBeltIdFromPickers();
                await _connection.UpdateAsync(_promotedPerson.Person);

                _list.Remove(_promotedPerson);
                await Navigation.PopModalAsync();
            }
        }

        #endregion

        #region Methods

        private async void SaveState()
        {
            if (pckrBelt.SelectedIndex != -1 && pckrStripes.SelectedIndex != -1)
            {
                var beltId = await GlobalMethods.DbHelper.GetChosenBeltId(_connection, pckrBelt.SelectedIndex, pckrStripes.SelectedIndex);
                _promotedPerson.NewBelt = await GlobalMethods.DbHelper.GetChosenBelt(_connection, beltId);
                _promotedPerson.CheckIfPromotionIsOK();
            }
        }

        private void SetPickers()
        {
            pckrBelt.SelectedIndex = (int)_promotedPerson.NewBelt.BeltColour;
            pckrStripes.SelectedIndex = (int)_promotedPerson.NewBelt.Stripes;
        }

        private async Task<int> GetBeltIdFromPickers()
        {
            return await GlobalMethods.DbHelper.GetChosenBeltId(_connection, pckrBelt.SelectedIndex, pckrStripes.SelectedIndex);
        }

        private async Task SetOldBeltInfo()
        {
            var belt = await GlobalMethods.DbHelper.GetChosenBelt(_connection, _promotedPerson.Person.BeltId);
            pckrOldBelt.SelectedIndex = (int)belt.BeltColour;
            pckrOldStripes.SelectedIndex = (int)belt.Stripes;
        }

        private bool IsImageVisible()
        {
            if (String.IsNullOrEmpty(_promotedPerson.Person.Photo))
            {
                this.PersonPhoto.IsVisible = false;
                return false;
            }
            else
            {
                this.PersonPhoto.IsVisible = true;
                return true;
            }
        }

        #endregion
    }
}