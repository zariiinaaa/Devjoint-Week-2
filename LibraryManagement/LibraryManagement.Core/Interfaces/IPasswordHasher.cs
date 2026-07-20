using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Core.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string password,string passwordHash);
}