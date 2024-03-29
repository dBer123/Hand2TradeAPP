﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Hand2TradeAP.ViewModels;


namespace Hand2TradeAP.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileView : ContentView
    {

      

        public ProfileView()
        {

            ProfileViewModel context = new ProfileViewModel();
            this.BindingContext = context;
            InitializeComponent();
            MyStar1.Text = context.Stars[0];
            MyStar2.Text = context.Stars[1];
            MyStar3.Text = context.Stars[2];
            MyStar4.Text = context.Stars[3];
            MyStar5.Text = context.Stars[4];



        }





        void OnRightButtonClicked(object sender, EventArgs e)
            => SideMenuView.State = SideMenuState.RightMenuShown;
       
        private void navigationDrawerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            navigationDrawerList.SelectedItem = null;
        }
    }
}

