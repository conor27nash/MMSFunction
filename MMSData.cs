using System;

namespace MMS.Function
{
    public class MMSData
    {
        public string Id { get; set; }
        public float Power { get; set; }   
        public float Rpm { get; set; }
        public float Temperature { get; set; }
        public DateTime DateLogged = DateTime.Now;
    }
}