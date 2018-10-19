using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommonLibrary
{
    public class User
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObjectId id;
        private string address;
        private bool available;
        private string role;


        [BsonId]
        public ObjectId Id { get=>id; set=>id=value; }

        [BsonElement("address")]
        public string Address { get => address; set => address = value; }

        [BsonElement("role")]
        public string Role { get => role; set => role = value; }

        [BsonElement("available")]
        public bool Available { get => available; set { available = value; OnPropertyChanged("Available"); } }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
