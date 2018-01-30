﻿using Newtonsoft.Json;
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
    public enum BjjEventType
    {
        AttendanceList,
        Promotion
    }

    [Table("BjjEvents")]
    public class BjjEvent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private string _eventName;
        private BjjEventType _eventType;
        private string _participantsBlob;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (_eventName == value) return;
                _eventName = value;

                OnPropertyChanged();
            }
        }

        //PersonId
        public string ParticipantsBlob
        {
            get { return _participantsBlob; }
            set
            {
                _participantsBlob = value;
                OnPropertyChanged();
            }
        }

        public BjjEventType EventType
        {
            get { return _eventType; }
            set
            {
                if (_eventType == value) return;
                _eventType = value;

                OnPropertyChanged();
            }
        }

        public BjjEvent()
        {
            ObservableCollection<int> temp = new ObservableCollection<int>();
            ParticipantsBlob = JsonConvert.SerializeObject(temp);
        }

        public BjjEvent(BjjEventType eventType)
        {
            ObservableCollection<int> temp = new ObservableCollection<int>();
            ParticipantsBlob = JsonConvert.SerializeObject(temp);

            EventType = eventType;
        }
    }
}
