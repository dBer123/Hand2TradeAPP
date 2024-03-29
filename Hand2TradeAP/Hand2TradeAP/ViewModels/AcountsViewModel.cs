﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Hand2TradeAP.Services;
using Hand2TradeAP.Views;
using Hand2TradeAP.Models;
using Xamarin.Essentials;
using System.Linq;
using System.Collections.ObjectModel;

namespace Hand2TradeAP.ViewModels
{
    class AcountsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region SearchText
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
            }
        }
        private string searchTextError;
        public string SearchTextError
        {
            get { return searchTextError; }
            set
            {
                searchTextError = value;
                OnPropertyChanged("SearchTextError");
            }
        }
        private bool showSearchTextError;

        public bool ShowSearchTextError
        {
            get { return showSearchTextError; }
            set
            {
                showSearchTextError = value;
                OnPropertyChanged("ShowSearchTextError");
            }
        }
        void ValidateDescription()
        {
            ShowSearchTextError = true;
            if (SearchText == null || SearchText == "")
                SearchTextError = "Input can not be empty";
            else
                ShowSearchTextError = false;
        }
        #endregion
        public ObservableCollection<User> SearchedAcounts { get; set; }
        public ICommand Search => new Command(SearchUser);
        async void SearchUser()
        {
            Hand2TradeAPIProxy proxy = Hand2TradeAPIProxy.CreateProxy();
            if (SearchText != null || SearchText != "")
            {
                IEnumerable<User> usersSearched = await proxy.SearchAcount(SearchText);
                if (usersSearched == null)
                {
                    await App.Current.MainPage.DisplayAlert("There is no user that fit your search", "", "OK");
                }
                else
                {
                    SearchedAcounts.Clear();
                    foreach (User u in usersSearched)
                    {
                        SearchedAcounts.Add(u);
                    }
                }
            }
        }

        public ICommand Promote => new Command<Object>(PromoteUser);
        public async void PromoteUser(Object obj)
        {
            if (obj is User)
            {
                User user = (User)obj;
                if (user.IsAdmin)
                {
                    await App.Current.MainPage.DisplayAlert("User is already an admin", "", "OK");
                }
                else if (user.IsBlocked)
                {
                    await App.Current.MainPage.DisplayAlert("You can not promote a blocked user", "", "OK");
                }
                else
                {
                    Hand2TradeAPIProxy proxy = Hand2TradeAPIProxy.CreateProxy();
                    bool succeeded = await proxy.Promote(user);
                    if (!succeeded)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Could not block user", "OK");
                    }
                }
            }
        }
        public ICommand Block => new Command<Object>(BlockUser);
        public async void BlockUser(Object obj)
        {
            if (obj is User)
            {
                User user = (User)obj;
                if (user.IsBlocked)
                {
                    await App.Current.MainPage.DisplayAlert("User is already blocked", "", "OK");
                }
                else if (user.IsAdmin)
                {
                    await App.Current.MainPage.DisplayAlert("You can not block an admin", "", "OK");
                }
                else
                {
                    Hand2TradeAPIProxy proxy = Hand2TradeAPIProxy.CreateProxy();
                    bool succeeded = await proxy.Block(user);
                    if (!succeeded)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Could not block user", "OK");
                    }
                }
            }
        }
        public AcountsViewModel()
        {
            SearchedAcounts = new ObservableCollection<User>();
        }
    }
}
