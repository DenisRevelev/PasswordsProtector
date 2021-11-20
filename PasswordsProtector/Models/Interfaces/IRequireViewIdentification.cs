using System;

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
