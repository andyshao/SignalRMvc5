using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRMvc5.Models
{
    /// <summary>
    /// Так как приложение чата оперирует пользователями, то создадим модель пользователей.
    /// </summary>
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}