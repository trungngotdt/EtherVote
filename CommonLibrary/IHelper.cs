using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface IHelper
    {
        /// <summary>
        /// Init and return Web3
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        Web3 GetWeb3(string link);

        /// <summary>
        /// Init and return Contract (must init web3)
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Contract GetContract(string abi, string address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFunc"></param>
        /// <returns></returns>
        Function GetFunction(string nameFunc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<HexBigInteger> GetGasAsync(string nameFunc, object[] para = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addressFrom"></param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<T> CallFunctionAsync<T>(string addressFrom, string nameFunc, object[] para = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addressFrom"></param>
        /// <param name="gas"></param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<T> CallFunctionAsync<T>(string addressFrom, HexBigInteger gas, string nameFunc, object[] para = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressFrom"></param>
        /// <param name="getGas"></param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom, HexBigInteger getGas, string nameFunc, object[] para = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressFrom">your wallet address</param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom, string nameFunc, object[] para = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addressFrom"></param>
        /// <param name="gas"></param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom, HexBigInteger gas, string nameFunc, params object[] para) where T : new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addressFrom">your wallet address</param>
        /// <param name="nameFunc"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom, string nameFunc, object[] para = null) where T : new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="byteCode"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<TransactionReceipt> DeployContractAsync(string abi, string byteCode, string address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        Task<bool> CheckUnlockAccountAsync(string address, string pass);
    }
}
