﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Hand2TradeAP.Services;
using Hand2TradeAP.Models;
using System.Linq;
using Hand2TradeAP.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Hand2TradeAP.ViewModels
{
    internal class ChatGroupsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        private readonly ChatService chatService;
        public ChatGroupsViewModel()
        {
            App theApp = (App)Application.Current;
            Groups = new ObservableCollection<TradeChat>();
            chatService = new ChatService();
            ConnectToChatService();
        }
        private async void ConnectToChatService()
        {
            await GetGroups();

            List<string> grs = new List<string>();
            foreach (var chat in Groups)
            {
                grs.Add(chat.ChatId.ToString());
            }
            //connect to server and register to all groups
            await chatService.Connect(grs.ToArray());
        }
        public ICommand GroupCommand => new Command(async () =>
        {
            if (SelectedGroup != null)
            {
                TradeChat chat = SelectedGroup;
                await App.Current.MainPage.Navigation.PushModalAsync(new ChatPage(chat,chatService));
                SelectedGroup = null;

            }
        });

        private async Task<bool> GetGroups()
        {
            Hand2TradeAPIProxy proxy = Hand2TradeAPIProxy.CreateProxy();
            List<TradeChat> userGroups = await proxy.GetGroups();
            Groups.Clear();
            foreach (TradeChat chat in userGroups)
            {
                chat.LastMessage = chat.TextMessages.OrderByDescending(m => m.SentTime).FirstOrDefault();
                Groups.Add(chat);
            }
            return true;
        }

        public ObservableCollection<TradeChat> Groups { get; set; }
  

        private TradeChat selectedGroup;
        public TradeChat SelectedGroup
        {
            get => selectedGroup;
            set
            {
                selectedGroup = value;
                OnPropertyChanged("SelectedGroup");
            }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }

        public ICommand RefreshCommand => new Command(() =>
        {
            IsRefreshing = true;
            GetGroups();
            IsRefreshing = false;
        });

    }
}
