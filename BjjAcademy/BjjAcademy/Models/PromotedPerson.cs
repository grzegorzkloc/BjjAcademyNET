using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.Models
{
    public class PromotedPerson : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private Person _person;
        private Belt _newBelt;
        private bool _isPromotionOK;

        public PromotedPerson(Person PersonToPromote, Belt NewBelt)
        {
            _person = PersonToPromote;
            _newBelt = NewBelt;
            IsPromotionOK = false;
        }

        public Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
            }
        }
        public Belt NewBelt
        {
            get { return _newBelt; }
            set { _newBelt = value; }
        }
        public bool IsPromotionOK
        {
            get { return _isPromotionOK; }
            set
            {
                if (_isPromotionOK == value) return;
                _isPromotionOK = value;
                OnPropertyChanged();
            }
        }

    }
}
