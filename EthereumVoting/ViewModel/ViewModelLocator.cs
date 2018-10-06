/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:EthereumVoting.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using EthereumVoting.Model;
using EthereumVoting.Utilities;
using EthereumVoting.Utilities.HelperMongo;
using System;

namespace EthereumVoting.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {

        const string link = "http://localhost:8545";

        const string abi = @"[{""constant"":false,""inputs"":[{""name"":""who"",""type"":""bytes32""}],""name"":""VoteFor"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""bytes32""}],""name"":""candidates"",""outputs"":[{""name"":""name"",""type"":""bytes32""},{""name"":""numVote"",""type"":""uint8""},{""name"":""isExist"",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""i"",""type"":""uint256""}],""name"":""GetAllCan"",""outputs"":[{""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""can"",""type"":""bytes32""}],""name"":""SetCandidate"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""GetVoterCount"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""listCan"",""outputs"":[{""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""address""}],""name"":""voters"",""outputs"":[{""name"":""availble"",""type"":""bool""},{""name"":""numVote"",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_voter"",""type"":""address""}],""name"":""SetVoter"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";


        const string byteCode = "60806040526002805460ff1916905534801561001a57600080fd5b506103aa8061002a6000396000f30060806040526004361061008d5763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416631925d44381146100925780631a0478d5146100ac578063649431e8146100e657806375ab6c2214610110578063805d9d4114610128578063841ada7814610153578063a3ec138d1461016b578063c6ff1274146101b6575b600080fd5b34801561009e57600080fd5b506100aa6004356101e4565b005b3480156100b857600080fd5b506100c4600435610242565b6040805193845260ff9092166020840152151582820152519081900360600190f35b3480156100f257600080fd5b506100fe600435610267565b60408051918252519081900360200190f35b34801561011c57600080fd5b506100aa60043561028a565b34801561013457600080fd5b5061013d6102fc565b6040805160ff9092168252519081900360200190f35b34801561015f57600080fd5b506100fe600435610305565b34801561017757600080fd5b5061019973ffffffffffffffffffffffffffffffffffffffff60043516610324565b60408051921515835260ff90911660208301528051918290030190f35b3480156101c257600080fd5b506100aa73ffffffffffffffffffffffffffffffffffffffff60043516610342565b60008181526020819052604090206001810154610100900460ff161561023e573360009081526003602090815260408083208590558483529082905290206001908101805460ff19811660ff918216909301169190911790555b5050565b6000602081905290815260409020805460019091015460ff8082169161010090041683565b600060018281548110151561027857fe5b90600052602060002001549050919050565b60008181526020819052604081208281556001908101805461010061ffff1990911617905580548082018255918190527fb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf6909101919091556002805460ff19811660ff91821690930116919091179055565b60025460ff1690565b600180548290811061031357fe5b600091825260209091200154905081565b60046020526000908152604090205460ff8082169161010090041682565b73ffffffffffffffffffffffffffffffffffffffff166000908152600460205260409020805461ff001960ff19909116600117166101001790555600a165627a7a72305820d6a0968aaee339a159d6570549056b7bdc869584586144090d5414a6cd1f3c7f0029";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ShellViewModel>();

            SimpleIoc.Default.Register<IHelper, Helper>();
            SimpleIoc.Default.Register<IHelperMongo, HelperMongo>();
            SimpleIoc.Default.Register<IGetMongoCollection, GetMongoCollection>();
            SimpleIoc.Default.Register<IRegisterParamaters, RegisterParamaters>();
            
            SimpleIoc.Default.Register<string>(()=>abi,"abi");
            SimpleIoc.Default.Register<string>(() => link, "link");
            SimpleIoc.Default.Register<string>(() => byteCode, "bytecode");

            SetupNavigation();
        }

        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Login", new Uri("../View/LoginWindow.xaml", UriKind.Relative));
            navigationService.Configure("Main", new Uri("../View/MainWindow.xaml", UriKind.Relative));
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the Login property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }


        /// <summary>
        /// Gets the Shell property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ShellViewModel Shell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ShellViewModel>();
            }
        }


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}