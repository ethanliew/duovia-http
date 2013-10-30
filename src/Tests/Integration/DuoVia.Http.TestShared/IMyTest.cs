using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoVia.Http.TestShared
{
    public interface IMyTest
    {
        string GetName(string key);
        string DoSomething(string work);
    }
}
