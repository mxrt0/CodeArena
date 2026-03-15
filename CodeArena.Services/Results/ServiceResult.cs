using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Results;

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public T? Data { get; private set; }

    public static ServiceResult<T> Ok(T data) 
        => new ServiceResult<T> { Success = true, Data = data };
    public static ServiceResult<T> Fail(string message) 
        => new ServiceResult<T> { Success = false, ErrorMessage = message };
}
