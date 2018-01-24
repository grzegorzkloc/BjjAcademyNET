using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BjjAcademy;
using SQLite;
using SQLite.Extensions;
using System.Collections.ObjectModel;
using BjjAcademy.GlobalMethods;
using Plugin.Media;
using PCLStorage;
using Plugin.Media.Abstractions;

namespace BjjAcademy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUpdatePersonPage : ContentPage
    {
        #region Variables

        private ObservableCollection<Person> PersonsList;
        private SQLiteAsyncConnection _connection;

        // Flags
        private bool IsNameOK;
        private bool IsSurnameOK;
        private bool IsPseudoOK;

        //Indicates if a page is an Add Person Page 
        //or an Update Person Page
        //IsAdd = true = AddPersonPage
        //IsAdd = false = UpdatePersonPage
        private bool IsAddPage;

        private string FilePath { get; set; }
        private string FilePathOldPhoto { get; set; }

        Person person;
        #endregion

        #region Constructor

        public AddUpdatePersonPage(SQLiteAsyncConnection _connection, ref ObservableCollection<Person> PersonsList, Person person = null)
        {
            this.person = person;

            IsNameOK = IsSurnameOK = false;
            IsPseudoOK = true;

            this.PersonsList = PersonsList;
            this._connection = _connection;
            BindingContext = this;
            InitializeComponent();
        }

        #endregion

        #region Overridding
        protected override async void OnAppearing()
        {
            if (person == null)
            {
                IsAddPage = true;
                Title = "Dodaj osobę";
                lblPageTitle.Text = "Dodaj osobę";
                this.CirclePhoto.IsVisible = false;
            }
            else
            {
                IsAddPage = false;
                this.Name.Text = person.Name;
                this.Surname.Text = person.Surname;
                this.Pseudo.Text = person.Pseudo;
                Belt belt = await _connection.GetAsync<Belt>(person.BeltId);
                this.pckrBelt.SelectedIndex = (int)belt.BeltColour;
                this.pckrStripes.SelectedIndex = (int)belt.Stripes;
                if (!String.IsNullOrEmpty(person.Photo))
                {
                    //this.FilePath = person.Photo;
                    this.btnPhotoMaker.Text = "Zmień zdjęcie";
                }

                if (String.IsNullOrEmpty(person.Photo)) this.CirclePhoto.IsVisible = false;
                else this.CirclePhoto.IsVisible = true;

                this.CirclePhoto.Source = person.Photo;
                this.AddBtn.Text = "Zapisz";
                Title = "Aktualizuj dane";
                lblPageTitle.Text = "Aktualizuj dane";
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion

        #region Events
        private void Name_Completed(object sender, EventArgs e)
        {
            this.IsNameOK = CheckEntry(sender as Entry, this.lblNameValidation);
            CheckFlags();
            this.Surname.Focus();
        }

        private void Surname_Completed(object sender, EventArgs e)
        {
            this.IsSurnameOK = CheckEntry(sender as Entry, this.lblSurnameValidation);
            CheckFlags();
            this.Pseudo.Focus();
        }

        private void Pseudo_Completed(object sender, EventArgs e)
        {
            this.IsPseudoOK = CheckEntry(sender as Entry, this.lblPseudoValidation);
            CheckFlags();
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsNameOK = CheckEntry(sender as Entry, this.lblNameValidation);
            CheckFlags();
        }

        private void Surname_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsSurnameOK = CheckEntry(sender as Entry, this.lblSurnameValidation);
            CheckFlags();
        }

        private void Pseudo_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsPseudoOK = CheckEntry(sender as Entry, this.lblPseudoValidation);
            CheckFlags();
        }

        private async void CancelBtn_Clicked(object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty(FilePath) && IsAddPage == true)
            if (!String.IsNullOrEmpty(FilePath))
            {
                var file = await FileSystem.Current.GetFileFromPathAsync(FilePath);
                await file.DeleteAsync();

            }
            await Navigation.PopModalAsync();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            if (await CheckIfPersonExists(this.person))
            {
                AddUpdatePerson();
                await Navigation.PopModalAsync();
            }
        }

        private void Pckr_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFlags();
        }

        private async void BtnPhotoMaker_Clicked(object sender, EventArgs e)
        {

            Image image = new Image();
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                Directory = "People",
            });

            if (file == null)
                return;

            ShowPhoto(file.Path);

            image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });
        }

        private async void BtnPhotoPicker_Clicked(object sender, EventArgs e)
        {
            Image image = new Image();

            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                PickMediaOptions options = new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                };

                var photo = await CrossMedia.Current.PickPhotoAsync(options);

                if (photo == null) return;

                //Function used only because PhotoPicker puts files into temp folder
                //and I did't like that (no control makes me insecure)
                string path = await DbHelper.PutFilesInCorrectDirectory(photo);

                ShowPhoto(path);
                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = photo.GetStream();
                    photo.Dispose();
                    return stream;
                });

            }
        }

        #endregion

        #region Methods

        private void AddUpdatePerson()
        {
            if (IsAddPage == true)
            {
                AddPerson();
            }
            else if (IsAddPage == false)
            {
                UpdatePerson();
            }
        }

        private bool CheckEntry(Entry field, Label lbl)
        {
            if (String.IsNullOrEmpty(field.Text))
            {
                if (field == this.Pseudo)
                {
                    lbl.Text = "";
                    return true;
                }

                lbl.Text = "Pole obowiązkowe";
                return false;
            }
            else if (field.Text.Length > 60)
            {
                lbl.TextColor = Color.Red;
                lbl.Text = "Max 60 znaków";
                return false;
            }
            else if (field.Text.Length == 0)
            {
                if (field == this.Pseudo)
                {
                    lbl.Text = "";
                    return true;
                }
                lbl.TextColor = Color.Red;
                lbl.Text = "Pole obowiązkowe";
                return false;
            }
            else
            {
                lbl.TextColor = Color.Green;
                lbl.Text = "OK";
                return true;
            }
        }

        private void CheckFlags()
        {
            if (IsNameOK && IsSurnameOK && IsPseudoOK
                && this.pckrBelt.SelectedIndex != -1 && this.pckrStripes.SelectedIndex != -1)
            {
                ActivateButtons();
            }
            else
            {
                DeactivateButtons();
            }
        }

        private async void AddPerson()
        {
            int beltid = await DbHelper.GetChosenBeltId(_connection, this.pckrBelt.SelectedIndex,
                this.pckrStripes.SelectedIndex);

            Person NewPerson = SetPersonDetails(this.Name.Text, this.Surname.Text,
                this.Pseudo.Text, beltid, FilePath);

            await this._connection.InsertAsync(NewPerson);
            this.PersonsList.Add(NewPerson);
            await DisplayAlert("Dodano osobę", "Osoba " + this.Name.Text + " " + this.Surname.Text + " dodana.", "OK");
        }

        private async void UpdatePerson()
        {
            int beltid = await DbHelper.GetChosenBeltId(_connection, this.pckrBelt.SelectedIndex,
                this.pckrStripes.SelectedIndex);

            int index = PersonsList.IndexOf(person);
            var UpdatedPerson = PersonsList[index];

            if (!String.IsNullOrEmpty(UpdatedPerson.Photo))
            {
                if (UpdatedPerson.Photo != FilePath && FilePath != null)
                {
                    var file = await PCLStorage.FileSystem.Current.GetFileFromPathAsync(UpdatedPerson.Photo);
                    await file.DeleteAsync();
                }
            }

            UpdatedPerson.Name = Name.Text.Trim();
            UpdatedPerson.Surname = Surname.Text.Trim();
            if (String.IsNullOrEmpty(Pseudo.Text)) UpdatedPerson.Pseudo = "";
            else UpdatedPerson.Pseudo = Pseudo.Text.Trim();
            UpdatedPerson.BeltId = (byte)beltid;
            if (!String.IsNullOrEmpty(FilePath)) UpdatedPerson.Photo = FilePath;

            await _connection.UpdateAsync(UpdatedPerson);

            await DisplayAlert("Uaktualniono osobę", "Osoba " + Name.Text + " " + Surname.Text + " uaktualniona.", "OK");
            MessagingCenter.Send(this, MessagingCenterMessage.PersonUpdated, PersonsList);
        }

        private async void ShowPhoto(string filePath)
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                FilePath = filePath;
                FilePathOldPhoto = filePath;
                this.CirclePhoto.Source = FilePath;
                this.btnPhotoMaker.Text = "Zmień zdjęcie";
            }
            else
            {
                FilePathOldPhoto = FilePath;
                FilePath = filePath;
                this.CirclePhoto.Source = FilePath;

                var file = await FileSystem.Current.GetFileFromPathAsync(FilePathOldPhoto);
                if (file != null && !String.Equals(FilePathOldPhoto, FilePath)) await file.DeleteAsync();
                FilePathOldPhoto = FilePath;
            }
            CirclePhotoVisibile();
        }

        private Person SetPersonDetails(string Name, string Surname, string Pseudo, int BeltId, string PhotoFilePath = null)
        {
            string pseudo = Pseudo;
            if (String.IsNullOrEmpty(pseudo)) pseudo = "";
            else pseudo.Trim();
            Person NewPerson = new Person
            {
                Name = Name.Trim(),
                Surname = Surname.Trim(),
                Pseudo = pseudo,
                BeltId = (byte)BeltId,
                Photo = PhotoFilePath
            };

            return NewPerson;
        }

        private void ActivateButtons()
        {
            this.AddBtn.IsEnabled = true;
            this.btnPhotoMaker.IsEnabled = true;
            this.btnPhotoPicker.IsEnabled = true;
        }

        private void DeactivateButtons()
        {
            this.AddBtn.IsEnabled = false;
            this.btnPhotoMaker.IsEnabled = false;
            this.btnPhotoPicker.IsEnabled = false;
        }

        private void CirclePhotoVisibile()
        {
            if (String.IsNullOrEmpty(FilePath)) this.CirclePhoto.IsVisible = false;
            else this.CirclePhoto.IsVisible = true;
        }

        private async Task<bool> CheckIfPersonExists(Person person = null)
        {
            foreach (Person ListedPerson in PersonsList)
            {
                string TempPseudo;
                if (String.IsNullOrEmpty(Pseudo.Text)) TempPseudo = "";
                else TempPseudo = Pseudo.Text.Trim();
                Person AboutToBeCreated = new Person()
                {
                    Name = Name.Text.Trim(),
                    Surname = Surname.Text.Trim(),
                    Pseudo = TempPseudo
                };

                if (ListedPerson.Name == AboutToBeCreated.Name && ListedPerson.Surname == AboutToBeCreated.Surname
                    && ListedPerson.Pseudo == AboutToBeCreated.Pseudo)
                {
                    //Check if saving data on existing person.
                    if (ListedPerson == person) return true;
                    await DisplayAlert("Błąd", "Osoba istnieje już w bazie danych!", "OK");
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}