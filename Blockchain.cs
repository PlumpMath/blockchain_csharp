using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace blockchain
{
    public class Blockchain
    {
        public Blockchain()
        {
            Chain = new List<Block>();
            CurrentTransactions = new List<Transaction>();
            Nodes = new HashSet<string>();

            NewBlock(previousHash:"1", proof:100);
        }

        public List<Transaction> CurrentTransactions {get;set;}
        public List<Block> Chain {get;set;}
        public HashSet<string> Nodes {get;set;}

        public Block LastBlock {
            get{
                return Chain.Last();
            }
        }

        private Block NewBlock(string previousHash, int proof)
        {
            Block block = new Block(){
                Index = Chain.Count + 1,
                Timestamp = DateTime.Now,
                Transactions = CurrentTransactions,
                Proof = proof,
                PreviousHash = previousHash ?? Hash(Chain[-1])
            };

            CurrentTransactions = new List<Transaction>();

            Chain.Add(block);

            return block;
        }

        public string Hash(Block block)
        {
            byte[] hash;
            string hashString;

            var blockString = Newtonsoft.Json.JsonConvert.SerializeObject(block);

            SHA256 mySHA256 = SHA256Managed.Create();
            hash = mySHA256.ComputeHash(System.Text.Encoding.Unicode.GetBytes(blockString));
            hashString = Encoding.Unicode.GetString(hash, 0, hash.Length);

            return hashString;
        }

        public bool ValidProof(int lastProof, int proof)
        {
            bool valid = false;
            SHA256 mySHA256 = SHA256Managed.Create();

            var guess = System.Text.Encoding.Unicode.GetBytes($"{lastProof}{proof}");
            var guessHash = mySHA256.ComputeHash(guess);

            valid = guessHash.ToString().Substring(0,4) == "0000";
            return valid;
        }

        public int ProofOfWork(int lastProof)
        {
            int proof = 0;
            while (ValidProof(lastProof, proof) == false)
            {
                proof++;
            }

            return proof;
        }

        public int NewTransaction(string sender, string recipient, int amount)
        {
            CurrentTransactions.Add(
                new Transaction(){
                    Sender = sender,
                    Recipient = recipient,
                    Amount = amount
                }
            );

            return LastBlock.Index + 1;
        }
    }

    public class Block
    {
        public int Index {get;set;}
        public DateTime Timestamp {get;set;}
        public List<Transaction> Transactions {get;set;}
        public int Proof {get;set;}
        public string PreviousHash {get;set;}
    }

    public class Transaction
    {
        public string Sender {get;set;}
        public string Recipient {get;set;}
        public int Amount {get;set;}
    }
}