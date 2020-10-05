using ServersInterrogator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersInterrogator.ViewModel
{
    public class ServerInfoViewModel : INotifyPropertyChanged
    {
        public ServerInfoViewModel()
        {
            ServerInfos = new ObservableCollection<ServerInfo>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ServerInfo> _serverInfos;
        public ObservableCollection<ServerInfo> ServerInfos
        {
            get
            {
                return _serverInfos;
            }
            set
            {
                _serverInfos = value;
                NotifyPropertyChanged(nameof(ServerInfos));
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
