﻿using BarabanPanel.Infrastructure.Commands;
using BarabanPanel.Infrastructure.Commands.Base;
using BarabanPanel.Models;
using BarabanPanel.Services;
using BarabanPanel.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BarabanPanel.ViewModels
{
    class GetInMelodyViewModel : ViewModelBase
    {
        private ViewModelMainWindow _MainViewModel;

        private JsonReader _reader;

        #region Атрибуты мдля мелодии

        private string _melodyName;
        public string MelodyName
        {
            get => _melodyName;

            set
            {
                if (value != null)
                    _melodyName = value;
            }
        }

        private Melody _currentMelody;
        public Melody CurrentMelody
        {
            get => _currentMelody;

            set
            {
                if (value != null)
                    _currentMelody = value;
            }
        }

        private IEnumerable<string> _melodies;

        public IEnumerable<string> Melodies
        {
            get
            {
                return _melodies;
            }
            set 
            {
                _melodies = value;
                OnPropertyChanged(nameof(Melodies));
            }
        }
        #endregion
        public CommandBase StartMelody { get; }
        private bool CanStartMelodyExecute(object p) => true;

        private void StartMelodyExecute(object melody)
        {
           if(MelodyName != null)
           {
                CurrentMelody = _reader.GetMelody(MelodyName);
                GetNextPartOfMelody();
                lastClickTime = DateTime.Now;
                _inRitm = true;
                UpdateTimeAsync();
            }
        }


        #region Процесс проигрыша мелодии

        private string _barabanName;
        private int _barabanParamNumber = 0;
        private DateTime lastClickTime;

        private bool _inRitm = false;

        private double _currentTemp;
        public double CurrentTemp
        {
            get
            {
                return _currentTemp;
            }
            set
            {
                    _currentTemp = value;
                    OnPropertyChanged(nameof(CurrentTemp)); 
            }
        }

        private double _currentTime;

        public double CurrentTime
        {
            get
            {
                return _currentTime;
            }
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        public CommandBase CheckTime { get; }
        private bool CanCheckTimeExecute(object p) => true;

        private void CheckTimeExecute(object barabanName)
        {
            if (barabanName != null)
            {
                if(barabanName.ToString() == _barabanName)
                {
                    TimeSpan elapsedTime = DateTime.Now - lastClickTime;

                    if (elapsedTime.TotalSeconds > _currentTemp + 0.5 || elapsedTime.TotalSeconds < _currentTemp - 0.5)
                    {
                        MessageBox.Show("Ты не попал в тайминг");
                        _inRitm = false;
                        _barabanParamNumber = 0;
    }
                    else
                    {
                        GetNextPartOfMelody();
                        lastClickTime = DateTime.Now;
                    }
                }
                else
                {
                    MessageBox.Show("Не тот барабан");
                    _inRitm = false;
                    _barabanParamNumber = 0;
                }

                
            }

        }

        private async Task UpdateTimeAsync()
        {
            while (_inRitm == true)
            {
                TimeSpan elapsedTime = DateTime.Now - lastClickTime;
                CurrentTime = elapsedTime.TotalSeconds;
                await Task.Delay(1);
            }

        }

        private void GetNextPartOfMelody()
        {
            if(_barabanParamNumber <= CurrentMelody.BarabanParts.Count -1)
            {
                CurrentTemp = CurrentMelody.BarabanParts[_barabanParamNumber].BarabanTaiming;
                _barabanName = CurrentMelody.BarabanParts[_barabanParamNumber].BarabanNumber;
                MessageBox.Show(CurrentTemp.ToString());
                _barabanParamNumber++;
            }
            else
            {
                MessageBox.Show("Мелодия пройдена");
                _inRitm = false;
            }
            
        }
        #endregion
        public GetInMelodyViewModel() : this(null)
        {
            JsonReader reader = new JsonReader();
            Melodies = reader.GetDictionaryNames();
            StartMelody = new RegularCommand(StartMelodyExecute, CanStartMelodyExecute);
        }
        public GetInMelodyViewModel(ViewModelMainWindow MainModel)
        {
            _MainViewModel = MainModel;
            _reader= new JsonReader();
            Melodies = _reader.GetDictionaryNames();
            StartMelody = new RegularCommand(StartMelodyExecute, CanStartMelodyExecute);
            CheckTime = new RegularCommand(CheckTimeExecute, CanCheckTimeExecute);
        }
    }
}
