using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.Models
{
    public class SelectedPerson : INotifyPropertyChanged
    {
        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private bool _isSelected;

        public Person Person { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;

                OnPropertyChanged();
            }
        }

        public SelectedPerson(Person person)
        {
            Person = person;
            IsSelected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
