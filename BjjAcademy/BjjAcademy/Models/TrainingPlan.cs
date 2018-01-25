using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.Models
{
    [Table("TrainingPlans")]
    public class TrainingPlan : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private string _name;
        private string _trainingActivitiesBlob;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

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

        public string TrainingActivitiesBlob
        {
            get { return _trainingActivitiesBlob; }
            set
            {
                if (_trainingActivitiesBlob == value) return;
                _trainingActivitiesBlob = value;

                OnPropertyChanged();
            }
        }

        public TrainingPlan()
        {
            ObservableCollection<string> temp = new ObservableCollection<string>();
            TrainingActivitiesBlob = JsonConvert.SerializeObject(temp);
        }
    }
}
