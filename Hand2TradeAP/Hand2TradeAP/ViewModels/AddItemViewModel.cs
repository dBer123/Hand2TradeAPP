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


namespace Hand2TradeAP.ViewModels
{
    class AddItemViewModel : INotifyPropertyChanged, IImageSourceUpdatable
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Description
        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                ValidateDescription();
                OnPropertyChanged("Description");
            }
        }

        private string descriptionError;
        public string DescriptionError
        {
            get { return descriptionError; }
            set
            {
                descriptionError = value;
                OnPropertyChanged("DescriptionError");
            }
        }
        private bool showDescriptionError;

        public bool ShowDescriptionError
        {
            get { return showDescriptionError; }
            set
            {
                showDescriptionError = value;
                OnPropertyChanged("ShowDescriptionError");
            }
        }
        void ValidateDescription()
        {
            ShowDescriptionError = true;
            if (Description == null || Description == "")
                PriceError = "Input can not be empty";
            else if (Description.Length > 220)
                DescriptionError = "Description must be under 220 notes";
            else
                ShowDescriptionError = false;
        }
        #endregion
        #region Price
        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                ValidatePrice();
                OnPropertyChanged("Price");
            }
        }

        private string priceError;
        public string PriceError
        {
            get { return priceError; }
            set
            {
                priceError = value;
                OnPropertyChanged("PriceError");
            }
        }
        private bool showPriceError;

        public bool ShowPriceError
        {
            get { return showPriceError; }
            set
            {
                showPriceError = value;
                OnPropertyChanged("ShowPriceError");
            }
        }
        void ValidatePrice()
        {
            ShowPriceError = true;
            if (Price == null || Price == "")
                PriceError = "Input can not be empty";
            else if (!Price.All(char.IsDigit))
                PriceError = "Enter only digits";
            else
                ShowPriceError = false;
        }
        #endregion
        #region ItemName
        private string itemName;
        public string Itemname
        {
            get { return itemName; }
            set
            {
                itemName = value;
                ValidateItemName();
                OnPropertyChanged("Itemname");
            }
        }

        private string itemNameError;
        public string ItemNameError
        {
            get { return itemNameError; }
            set
            {
                itemNameError = value;
                OnPropertyChanged("ItemNameError");
            }
        }
        private bool showItemNameError;

        public bool ShowItemNameError
        {
            get { return showItemNameError; }
            set
            {
                showItemNameError = value;
                OnPropertyChanged("ShowItemNameError");
            }
        }
        void ValidateItemName()
        {
            ShowItemNameError = true;
            if (Itemname == null || Itemname == "")
                ItemNameError = "Input can not be empty";
            else if (Itemname.Length > 20)
                ItemNameError = "Description must be under 20 notes";
            else
                ShowItemNameError = false;
        }
        #endregion
        private bool showimageError;

        public bool ShowimageError
        {
            get => showimageError;
            set
            {
                showimageError = value;
                OnPropertyChanged("ShowimageError");
            }
        }
        private string imageError;

        public string ImageError
        {
            get => imageError;
            set
            {
                imageError = value;
                OnPropertyChanged("ImageError");
            }
        }
        private void ValidateImage()
        {
            ShowimageError = true;
            if (imageFileResult == null)
                ImageError = "You must Add a picture of the item";

            else
                ShowimageError = false;
        }
        public async void UpdateImageSource(string imgSource)
        {
            this.imageFileResult = new FileResult(imgSource);
            var stream = await imageFileResult.OpenReadAsync();
            ImageSource source = ImageSource.FromStream(() => stream);
            if (this.SetImageSourceEvent != null)
                this.SetImageSourceEvent(source);
        }

        public ICommand NevigateBack => new Command(Back);
        async void Back()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

        public FileResult imageFileResult;
        public event Action<ImageSource> SetImageSourceEvent;
        public ICommand PickImageCommand => new Command(OnPickImage);
        public async void OnPickImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions()
                {
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    this.imageFileResult = result;
                    await App.Current.MainPage.Navigation.PushModalAsync(new CropImage(this));
                }

            }
            catch { }
            
        }
        public string ImgSource
        {
            get
            {
                if (this.imageFileResult != null)
                    return this.imageFileResult.FullPath;
                else
                    return string.Empty;
            }
            
        }
        ///The following command handle the take photo button
        public ICommand CameraImageCommand => new Command(OnCameraImage);
        public async void OnCameraImage()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();
                if (result != null)
                {
                    this.imageFileResult = result;
                    await App.Current.MainPage.Navigation.PushModalAsync(new CropImage(this));
                    //if (SetImageSourceEvent != null)
                    //    SetImageSourceEvent(result.FullPath);
                }
            }
            catch{}
            
        }

        
        private bool ValidateForm()
        {
            //Validate all fields first
            ValidateItemName();
            ValidatePrice();
            ValidateDescription();
            ValidateImage();

            //check if any validation failed
            if (ShowDescriptionError ||
                ShowItemNameError || ShowPriceError || ShowimageError)
                return false;
            return true;
        }

        public ICommand AddNewItem => new Command(SaveData);
        public async void SaveData()
        {
            if (ValidateForm())
            {
                Item item = new Item
                {
                    Desrciption = Description,
                    Price = int.Parse(Price),
                    ItemName = Itemname
                };
                             
                Hand2TradeAPIProxy proxy = Hand2TradeAPIProxy.CreateProxy();
                Item itemAdded = await proxy.AddItem(item);
                if (itemAdded == null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Can not add item", "OK");
                }
                else
                {
                    if (this.imageFileResult != null)
                    {
                       

                        bool success = await proxy.UploadImage(new FileInfo()
                        {
                            Name = this.imageFileResult.FullPath
                        }, $"I{itemAdded.ItemId}.jpg");
                    }
                    
                    App theApp = (App)Application.Current;
                    theApp.CurrentUser.Items.Add(itemAdded);
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
            }
          
        }
        public AddItemViewModel()
        {

        }
        
        
    }
}
