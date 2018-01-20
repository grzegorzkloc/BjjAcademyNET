using SQLite;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BjjAcademy
{
    [Table("Students")]
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Person(string Name, string Surname, string Pseudo, int BeltId)
        {
            this._name = Name;
            this._surname = Surname;
            this._pseudo = Pseudo;
            this._beltid = (byte)BeltId;

        }
        public Person()
        {

        }

        #region Private Fields
        private string _name;
        private string _surname;
        private string _pseudo;
        private string _photo;
        private byte _beltid;
        #endregion 

        #region Properties 
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(60), NotNull]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;

                OnPropertyChanged();
            }
        }

        [MaxLength(60), NotNull]
        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname == value) return;
                _surname = value;

                OnPropertyChanged();
            }
        }

        [MaxLength(60)]
        public string Pseudo
        {
            get { return _pseudo; }
            set
            {
                if (_pseudo == value) return;
                _pseudo = value;

                OnPropertyChanged();
            }
        }

        public string Photo
        {
            get { return _photo; }
            set
            {
                if (_photo == value) return;
                _photo = value;

                OnPropertyChanged();
            }
        }

        [ForeignKey(typeof(Belt))]
        public byte BeltId
        {
            get { return _beltid; }
            set
            {
                if (_beltid == value) return;
                _beltid = value;

                OnPropertyChanged();
            }
        }

        #endregion

        //TODO: Add date of last update
        //TODO: Add achievements
    }
}
