using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BjjAcademy.GlobalMethods;
using PCLStorage;

namespace BjjAcademy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonDetails : ContentPage
    {
        #region Variables

        Person student;
        SQLiteAsyncConnection _connection;
        ObservableCollection<Person> PersonsList;
        string PageTitle;

        #endregion

        #region Constructors

        public PersonDetails()
        {
            InitializeComponent();
        }
        public PersonDetails(ref Person person, SQLiteAsyncConnection _connection, ref ObservableCollection<Person> PersonsList)
        {
            student = person;
            this._connection = _connection;
            this.PersonsList = PersonsList;
            PageTitle = student.Name + " " + student.Surname;
            Title = PageTitle;
            InitializeComponent();
            DisplayDetails();
        }

        #endregion

        #region Events

        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Uwaga", "Czy jesteś pewny, że chcesz skasować osobę: " + student.Name + " " + student.Surname, "Kasuj", "Anuluj"))
            {
                if (!String.IsNullOrEmpty(student.Photo))
                {
                    var file = await FileSystem.Current.GetFileFromPathAsync(student.Photo);
                    if (file != null) await file.DeleteAsync();
                }
                await _connection.DeleteAsync(student);
                PersonsList.Remove(student);
                await Navigation.PopAsync();
            }
        }

        private async void UpdateBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddUpdatePersonPage(_connection, ref PersonsList, student));
            Navigation.RemovePage(this);
        }

        #endregion

        #region Methods

        public async void DisplayDetails()
        {
            if (IsImageVisible()) this.PersonPhoto.Source = student.Photo;
            this.NameSurnameLbl.Text = student.Name + " " + student.Surname;
            this.PseudoLbl.Text = student.Pseudo;
            this.BeltStripesLbl.Text = await DbHelper.GetBeltAndStripeInfo(_connection, student.BeltId);
        }

        private bool IsImageVisible()
        {
            if (String.IsNullOrEmpty(student.Photo))
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