using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumVoting.Utilities
{
    public class Helper : IHelper
    {
        private Web3 web3;
        private Contract contract;

        public Web3 Web30 { get => web3; set => web3 = value; }
        public Contract Contracts { get => contract; set => contract = value; }

        public async Task<bool> CheckUnlockAccountAsync(string address, string pass)
        {
            try
            {
                return await Web30.Personal.UnlockAccount.SendRequestAsync(address, pass, 60);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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

        public async Task<TransactionReceipt> DeployContractAsync(string abi, string byteCode, string address)
        {
            try
            {

                var result = await Web30.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(abi, byteCode, address, new HexBigInteger(900000));
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Contract GetContract(string abi, string address)
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
            try
            {
                return Contracts.GetFunction(nameFunc);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<HexBigInteger> GetGasAsync(string nameFunc, object[] para = null)
        {
            try
            {
                var gas = await GetFunction(nameFunc).EstimateGasAsync(para);
                return gas;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<T> CallFunctionAsync<T>(string addressFrom,HexBigInteger gas, string nameFunc, object[] para = null)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    
                    var result = await Contracts.GetFunction(nameFunc).CallAsync<T>(addressFrom, gas, null, functionInput: para);
                    return result;
                }
                catch (Exception ex)
                {
                    if (count == 5)
                    {
                        throw ex;
                    }

                }
                count++;
            }

        }

        public async Task<T> CallFunctionAsync<T>(string addressFrom, string nameFunc, object[] para = null)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    var gas = await GetGasAsync(nameFunc, para);
                    var result = await Contracts.GetFunction(nameFunc).CallAsync<T>(addressFrom, gas, null, functionInput: para);
                    return result;
                }
                catch (Exception ex)
                {
                    if (count == 5)
                    {
                        throw ex;
                    }

                }
                count++;
            }

        }

        public async Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom,HexBigInteger getGas, string nameFunc, object[] para = null)
        {
            int count = 0;
            while (true)
            {
                try
                {                    
                    var result = await GetFunction(nameFunc).SendTransactionAndWaitForReceiptAsync(from: addressFrom, gas: getGas, value: null, functionInput: para[0]);
                    return result;
                }
                catch (Exception ex)
                {
                    if (count == 5)
                    {
                        throw ex;
                    }
                    count++;
                }
            }
        }

        public async Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom, string nameFunc, object[] para = null)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    var getGas = await GetGasAsync(nameFunc, para);
                    
                    var result = await GetFunction(nameFunc).SendTransactionAndWaitForReceiptAsync(from: addressFrom, gas: getGas, value: null, functionInput: para[0]);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if(count==5)
                    {
                        throw ex;
                    }
                    count++;
                }
            }
        }

        public async Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom,HexBigInteger gas, string nameFunc, params object[] para) where T : new()
        {
            int count = 0;
            while (true)
            {
                try
                {
                    var result = await GetFunction(nameFunc).CallDeserializingToObjectAsync<T>(addressFrom, gas, null, functionInput: para);
                    return result;
                }
                catch (Exception ex)
                {
                    DebugThrow(nameFunc, ex, count);
                    if (count == 5)
                    {
                        throw ex;

                    }
                }
                count++;
            }
        }

        public async Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom, string nameFunc, params object[] para) where T : new()
        {
            int count = 0;
            while (true)
            {
                try
                {
                    var gas = await GetGasAsync(nameFunc, para);
                    var result = await GetFunction(nameFunc).CallDeserializingToObjectAsync<T>(addressFrom, gas, null, functionInput: para);
                    return result;
                }
                catch (Exception ex)
                {
                    DebugThrow(nameFunc, ex, count);                    
                    if(count==5)
                    {                        
                        throw ex;
                    }                    
                }
                count++;
            }
        }

        private void DebugThrow(string message,Exception ex,int countRetry=0)
        {
            string error = message + ex.Message+(countRetry>0? (" Retry : "+countRetry):"");
            Debug.WriteLine(error);
        }
    }
}
