﻿using CommonLibrary;
using CommonLibrary.HelperMongo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;

namespace AdminVoting.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        int[] indexUserBeChanged;
        int countUserBeChanged;

        private bool isOpenDialogAddUser;
        private object contentDialogAddUser;
        private bool isOpenSbNotify;
        private string messageSbNotify;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;
        private IGetMongoCollection getMongoCollection;

        private ObservableCollection<User> allUser;

        private ICommand commandOpenDialogAddUser;

        private ICommand commandChecked;
        private ICommand commandLoaded;
        private ICommand commandBtnSubmitedClick;

        private ICommand commandBtnAcceptDialogAddUser;
        private ICommand commandBtnCancelDialogAddUser;

        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => { GetAllUser(); });



        public IHelper Helper { get => helper; set => helper = value; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }
        public ObservableCollection<User> AllUser { get => allUser; set { allUser = value; RaisePropertyChanged("AllUser"); } }
        public IGetMongoCollection GetMongoCollection { get => getMongoCollection; set => getMongoCollection = value; }
        public ICommand CommandBtnSubmitedClick => commandBtnSubmitedClick = new RelayCommand(() => 
        {
            CheckException(() => {SubmitUser(); });            
        });
        public ICommand CommandChecked => commandChecked = new RelayCommand<string>((address) => { ToggleCheck(address); });

        public bool IsOpenDialogAddUser { get => isOpenDialogAddUser; set { isOpenDialogAddUser = value; RaisePropertyChanged("IsOpenDialogAddUser"); } }
        public ICommand CommandOpenDialogAddUser => commandOpenDialogAddUser = new RelayCommand(() => { OpenDialogAddUser(); });

        public object ContentDialogAddUser { get => contentDialogAddUser; set { contentDialogAddUser = value; RaisePropertyChanged("ContentDialogAddUser"); } }

        public ICommand CommandBtnAcceptDialogAddUser => commandBtnAcceptDialogAddUser = new RelayCommand<object[]>((obj) => 
        {
            CheckException(()=> { AcceptAddUser(obj);});            
        });
        public ICommand CommandBtnCancelDialogAddUser => commandBtnCancelDialogAddUser = new RelayCommand(() => { CancelAddUser(); });

        public bool IsOpenSbNotify { get => isOpenSbNotify; set => isOpenSbNotify = value; }
        public string MessageSbNotify { get => messageSbNotify; set => messageSbNotify = value; }





        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
            Init();

        }

        private void Init()
        {
            HelperMongo.GetClient("127.0.0.1", 27017, "user1", "pass1");
            var database = HelperMongo.GetDatabase("data1");
            getMongoCollection = HelperMongo.GetMongoCollection();
            getMongoCollection.Init(database, "user", typeof(User));
            AllUser = new ObservableCollection<User>();
            countUserBeChanged = 0;

        }

        private void GetAllUser()
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Empty;
            AllUser = new ObservableCollection<User>(GetMongoCollection.GetData(filter).Cast<User>());
            indexUserBeChanged = Enumerable.Repeat(-1, AllUser.Count).ToArray();
        }

        private void ToggleCheck(string address)
        {
            var index = AllUser.IndexOf(AllUser.AsParallel().First(x => x.Address.Equals(address)));
            if (!indexUserBeChanged.Contains(index))
            {
                indexUserBeChanged[countUserBeChanged] = index;
                countUserBeChanged++;
            }
            else
            {
                indexUserBeChanged[indexUserBeChanged.ToList().IndexOf(index)] = -1;
            }
        }

        private void SubmitUser()
        {
            var tempUser = new List<User>();
            indexUserBeChanged.AsParallel().ForAll(x =>
            {
                if (x != -1)
                {
                    tempUser.Add(AllUser[x]);
                }
            });

            for (int i = 0; i < indexUserBeChanged.Count(); i++)
            {
                if (indexUserBeChanged[i] != -1)
                {
                    var previous = Builders<User>.Filter.Eq("address", AllUser[indexUserBeChanged[i]].Address);
                    var update = Builders<User>.Update.Set("available", AllUser[indexUserBeChanged[i]].Available);
                    getMongoCollection.FindOneAndUpdateAsync(previous, update);
                }
            }

        }


        private void OpenDialogAddUser()
        {
            ContentDialogAddUser = new View.AddUserWindow();
            IsOpenDialogAddUser = true;
        }

        private void AcceptAddUser(object[] paras)
        {
            if (paras[0].ToString().Trim().Count() == 0 || paras[1].ToString().Trim().Count() == 0)
            {
                return;
            }
            getMongoCollection.InserOne(new User() { Address = paras[0].ToString(), Available = bool.Parse(paras[2].ToString()), Role = paras[1].ToString(), VoteFor = string.Empty });
            CancelAddUser();
        }

        private void CancelAddUser()
        {
            IsOpenDialogAddUser = false;
            OpenSnackBarNotify(false, "");
        }

        private void OpenSnackBarNotify(bool isOpen, string message)
        {
            IsOpenSbNotify = isOpen;
            MessageSbNotify = message;
        }

        private void CheckException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                OpenSnackBarNotify(true, ex.Message);
                throw;
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}