using CommonLibrary;
using CommonLibrary.HelperMongo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;
        private IGetMongoCollection getMongoCollection;

        private ObservableCollection<User> allUser;

        private ICommand commandChecked;
        private ICommand commandLoaded;
        private ICommand commandBtnSubmitedClick;


        public ICommand CommandLoaded => commandLoaded = new RelayCommand(()=> { GetAllUser(); });



        public IHelper Helper { get => helper; set => helper = value; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }
        public ObservableCollection<User> AllUser { get => allUser; set { allUser = value; RaisePropertyChanged("AllUser"); } }
        public IGetMongoCollection GetMongoCollection { get => getMongoCollection; set => getMongoCollection = value; }
        public ICommand CommandBtnSubmitedClick => commandBtnSubmitedClick = new RelayCommand(() => { SubmitUser(); });
        public ICommand CommandChecked => commandChecked = new RelayCommand<string>((address) => { ToggleCheck(address); });





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
            HelperMongo.GetDatabase("data1");
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
            AllUser = new ObservableCollection<User>(GetMongoCollection.GetData(filter).Cast <User>());
            indexUserBeChanged = Enumerable.Repeat(-1, AllUser.Count).ToArray(); 
        }

        private void ToggleCheck(string address)
        {

            var index = AllUser.IndexOf(AllUser.AsParallel().First(x => x.Address.Equals(address)));
            if(!indexUserBeChanged.Contains(index))
            {
                indexUserBeChanged[countUserBeChanged] = index;
                countUserBeChanged++;
            }
            else
            {
                indexUserBeChanged[ indexUserBeChanged.ToList().IndexOf(index)]=-1;
            }
            
        }

        private void SubmitUser()
        {
            var tempUser =new List<User>();
            indexUserBeChanged.AsParallel().ForAll(x => 
            {
                if (x!=-1)
                {
                    tempUser.Add(AllUser[x]);
                }
            });
            
            for (int i = 0; i < indexUserBeChanged.Count(); i++)
            {                
                if (indexUserBeChanged[i]!=-1)
                {
                    var previous = Builders<User>.Filter.Eq("address", AllUser[indexUserBeChanged[i]].Address);
                    var update = Builders<User>.Update.Set("available", AllUser[indexUserBeChanged[i]].Available);
                    getMongoCollection.FindOneAndUpdateAsync(previous, update);


                }
            }
            
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}