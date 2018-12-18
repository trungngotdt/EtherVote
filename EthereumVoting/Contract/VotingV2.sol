pragma solidity >=0.4.22 <0.6.0;

contract Voting
{
    
    mapping(bytes32 => candidate) public candidates;
    struct candidate
    {
        bytes32 name;
        uint8 numVote;
        bool isExist;
    }
    bytes32[] public listCan ;
    
    uint8 countCan=0;
    
    mapping(address=>bytes32[]) voted;
    
    function VoteFor(bytes32 who) public
    {
        
        if(candidates[who].isExist)
        {
            voted[msg.sender].push(who);
            candidates[who].numVote=candidates[who].numVote+1;
        }
        
    }
    
    function SetCandidate(bytes32 can) public
    {
        candidates[can].name=can;
        candidates[can].numVote=0;
        candidates[can].isExist=true;
        listCan.push(can);
        countCan++;
    }
    
    function GetAllCan(uint i) public returns(bytes32)
    {
        return listCan[i];
    }
    
    function GetVoterCount() public returns (uint8)
    {
        return countCan;
    }
    
}