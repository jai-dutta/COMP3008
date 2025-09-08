using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace DTOS
{
    public class DataStructDto
    {
        [Required]
        public string fname { get; set; }
        [Required]
        public string lname { get; set; }
        [Required]
        public uint pin { get; set; }
        [Required]
        public uint acct { get; set; }
        [Required]
        public int bal { get; set; }
        public static DataStructDto MapDataStructDto(DataStruct ds)
        {
            return new DataStructDto
            {
                fname = ds.firstName, lname = ds.lastName,
                pin = ds.pin, acct = ds.acctNo, bal = ds.balance
            };
        }
    }


}
