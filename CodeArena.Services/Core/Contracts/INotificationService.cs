using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface INotificationService
{
    Task SendMessageAsync(string messageName, object payload);
}
