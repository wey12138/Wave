﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    [DataContract(Name = "response")]
    public class SearchableResponse : Dictionary<string, List<int>>
    {
    }
}