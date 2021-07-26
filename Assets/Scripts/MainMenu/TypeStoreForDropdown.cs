using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class TypeStoreForDropdown : MonoBehaviour
    {
        public List<Type> Types;
        private void Start()
        {
            Types = new List<Type>();
        }
    }
}