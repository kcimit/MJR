using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MJR
{
    public class ViewModel : INotifyPropertyChanged
    {
        static SynchronizationContext uiSynchronizationContext;
        private CollectionView _lvItems;
        string _status, _inputFile, _outputFolder;
        Boolean inputEnabled;
        private int _maxJPEGSize;
        ObservableCollection<ItemVM> _progress;

        public int MaxJPEGSize
        {
            get => _maxJPEGSize;
            set
            {
                if (_maxJPEGSize == value) return;
                _maxJPEGSize = value;
                OnPropertyChanged("MaxJPEGSize");
                _progress= new ObservableCollection<ItemVM>();
                _lvItems = new CollectionView(_progress);
            }
        }

        public string InputFile
        {
            get => _inputFile; 
            set
            {
                if (_inputFile == value) return;
                _inputFile = value;
                OnPropertyChanged("InputFile");
            }
        }

        public string OutputFolder
        {
            get => _outputFolder; 
            set
            {
                if (_outputFolder == value) return;
                _outputFolder = value;
                OnPropertyChanged("OutputFolder");
            }
        }

        public string Status
        {
            get => _status; 
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public ViewModel()
        {
            uiSynchronizationContext = SynchronizationContext.Current;
            InputEnabled = true;
            Status = "Ready.";
        }

        public CollectionView FileProgress
        {
            get { return _lvItems; }
        }

        internal void AddProgress(ItemVM add)
        {
            _progress.Add(add);
            OnPropertyChanged(nameof(FileProgress));
        }

        public Boolean InputEnabled
        {
            get { return inputEnabled; }
            set
            {
                inputEnabled = value;
                OnPropertyChanged(nameof(InputEnabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            uiSynchronizationContext.Post(
                o => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
                , null
            );
        }

        internal void AssignProgress(IList<ItemVM> allResults)
        {
            _lvItems = new CollectionView(allResults);
            OnPropertyChanged(nameof(FileProgress));
        }
    }

    public class ItemVM
    {
        string _text;
        bool _bold;
        Color _background, _foreground;

        public ItemVM(string text, Color fore, Color back, bool bold)
        {
            _text = text;
            _foreground = fore;
            _background = back;
            _bold = bold;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
            }
        }

        public Color BackgroundColor
        {
            get { return _background; }
            set
            {
                if (value == _background) return;
                _background = value;
            }
        }

        public Color ForegroundColor
        {
            get { return _foreground; }
            set
            {
                if (value == _foreground) return;
                _foreground = value;
            }
        }

        public FontWeight Weight
        {
            get { return _bold ? FontWeights.Bold : FontWeights.Normal; }
            set { _bold = true ? value == FontWeights.Bold : false; }
        }
    }
}
