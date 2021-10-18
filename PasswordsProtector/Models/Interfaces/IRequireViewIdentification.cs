using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordsProtector.Models.Interfaces
{
    public interface IRequireViewIdentification
    {
        /// <summary>
        /// Unique identifier for the window.
        /// </summary>
        Guid ViewId { get; }
    }
}
