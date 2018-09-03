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
    public interface IHelper
    {
        Web3 GetWeb3(string link);

        Contract GetContract(string abi, string address);

        Function GetFunction(string nameFunc);

        Task<HexBigInteger> GetGasAsync(string nameFunc, object[] para = null);

        Task<T> CallFunctionAsync<T>(string addressFrom, string nameFunc, object[] para = null);

        Task<TransactionReceipt> SendTransactionFunctionAsync(string addressFrom, string nameFunc, object[] para = null);

        Task<T> GetCallDeserializingToObjectAsync<T>(string addressFrom, string nameFunc, object[] para = null) where T : new();
    }
}
