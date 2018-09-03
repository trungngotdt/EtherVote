using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumVoting.Utilities
{
    public class Helper:IHelper
    {
        private Web3 web3;
        private Contract contract;

        public Web3 Web30 { get => web3; set => web3 = value; }
        public Contract Contracts { get => contract; set => contract = value; }

        public Web3 GetWeb3(string link)
        {
            try
            {
                Web30 = new Web3(link);
                return Web30;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public Contract GetContract(string abi,string address)
        {
            try
            {
                Contracts = Web30.Eth.GetContract(abi, address);
                return Contracts;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public Function GetFunction(string nameFunc)
        {
            return Contracts.GetFunction(nameFunc);
        }

        public async Task<HexBigInteger> GetGasAsync(string nameFunc, object[] para = null)
        {
            var gas=await GetFunction(nameFunc).EstimateGasAsync(para);
            return gas;
        }

        public async Task<T> CallFunctionAsync<T>(string addressFrom,string nameFunc,object[] para=null)
        {
            var gas = await GetGasAsync(nameFunc, para);
            var result= await Contracts.GetFunction(nameFunc).CallAsync<T>(addressFrom, gas, null, functionInput: para);
            return result;
        }

        public async Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom,string nameFunc, object[] para = null)
        {
            var gas = await GetGasAsync(nameFunc, para);
            var result =await GetFunction(nameFunc).SendTransactionAndWaitForReceiptAsync(addressFrom,gas, null, functionInput: para);
            return result;
        }

        public async Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom, string nameFunc, object[] para = null) where T:new()
        {
            var gas = await GetGasAsync(nameFunc, para);
            var result = await GetFunction(nameFunc).CallDeserializingToObjectAsync<T>(addressFrom, gas, null, functionInput: para);
            return result;
        }
    }
}
