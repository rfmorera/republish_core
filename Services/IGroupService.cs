using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGroupService
    {
        Task Publish(string GroupId);
    }
}
