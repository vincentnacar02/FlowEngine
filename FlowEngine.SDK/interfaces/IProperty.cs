﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK.interfaces
{
    public interface IProperty
    {
        string getName();

        object getValue();

        void setValue(object value);
    }
}
