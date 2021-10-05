using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordsProtector.Models.Interfaces
{
    public interface IRequireViewIdentification
    {
        Guid ViewId { get; }
    }
}
