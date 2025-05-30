using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterfaceMethod
{
    public interface TableData
    {
        public int INDEX { get; set; }
    }

    public interface IItemData
    {
        public ItemData ItemData
        {
            get;
            set;
        }
    }
}
