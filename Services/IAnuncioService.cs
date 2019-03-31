using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IAnuncioService
    {
        void Publish(string url);
    }
}
