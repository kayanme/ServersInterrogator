using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServersInterrogator.Model
{
    public class ServerInfo : INotifyPropertyChanged
    {
        private readonly int _interval;

        public ServerInfo(string url, int interval)
        {
            Url = url;
            _interval = interval;
            StartConnectingTime = DateTime.Now;
            EndConnectingTime = DateTime.Now;
        }

        public string Url { get; }

        private bool _isConnecting;
        public bool IsConnecting
        {
            get
            {
                return _isConnecting;
            }
            set
            {
                _isConnecting = value;
                OnPropertyChanged(nameof(IsConnecting));
            }
        }

        private HttpStatusCode _code;
        public String StatusCode
        {
            get
            {
                return _code.ToString();
            }
            set
            {
                _code = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value);//такие-то вещи всё-таки конверторами обычно делаются в wpf. А зачем здесь кстати set?
                OnPropertyChanged(nameof(StatusCode));
            }
        }

        private DateTime _startConnectingTime;
        public DateTime StartConnectingTime
        {
            get
            {
                return _startConnectingTime;
            }
            set
            {
                _startConnectingTime = value;
                OnPropertyChanged(nameof(StartConnectingTime));
                OnPropertyChanged(nameof(ConnectionTime));
            }
        }

        private DateTime _endConnectingTime;
        public DateTime EndConnectingTime
        {
            get
            {
                return _endConnectingTime;
            }
            set
            {
                _endConnectingTime = value;
                OnPropertyChanged(nameof(EndConnectingTime));
                OnPropertyChanged(nameof(ConnectionTime));
            }
        }
        public double ConnectionTime => (EndConnectingTime - StartConnectingTime).TotalSeconds;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public async Task RequestAsync()//я бы выделил этот метод в отдельный сервис. Это у тебя не модель, это часть вьюмодели, по сути. Пусть она заполняется отдельно.
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Timeout = _interval;
            HttpWebResponse response = null;
            IsConnecting = true;
            StartConnectingTime = DateTime.Now;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch { }//сие всегда плохо. Хоть в лог что ли напиши.
            finally
            {
                if (response != null)
                {
                    response.Close();
                    StatusCode = response.StatusCode.ToString();
                }
                EndConnectingTime = DateTime.Now;
                IsConnecting = false;
            }
        }

    }
}
