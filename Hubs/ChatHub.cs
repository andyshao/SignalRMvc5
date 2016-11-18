using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRMvc5.Models;

namespace SignalRMvc5.Hubs
{
    /// <summary>
    /// SignalR использует две модели взаимодействия сервера и клиента: Persistent Connection и хабы. 
    /// В данном случае мы используем хабы. 
    /// Для этого создаем свой хаб ChatHub, который наследуется от класса Hub.
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// Во-первых, мы создаем список, который будет хранить подключенных к чату пользователей.
        /// </summary>
        static List<User> Users = new List<User>();

        /// <summary>
        ///  Отправка сообщений (всем пользователям от имени указанного пользователя)
        /// </summary>
        /// <param name="name">От кого посылаем сообщение</param>
        /// <param name="message">Текст сообщения</param>
        public void Send(string name, string message)
        {
            //++
            DateTime dtNow = DateTime.Now;
            string sTime = " - " + dtNow.ToLongTimeString() + " - ";

            // Объект Clients означает коллекцию всех пользователей хаба. 
            // Свойство All, идущее далее, говорит о том, что метод надо применить у всех подключенных клиентов.
            //
            // Далее в выражении следует метод addMessage. 
            // Этот метод объявляется на стороне клиента в коде javascript. 
            // Чуть позже мы добавим код клиентской стороны. 
            // А пока просто надо знать, что эти методы находятся не на серверной части, а на стороне клиента.
            //
            Clients.All.addMessage(name, message); // sTime + message
        }

        /// <summary>
        /// Подключение нового пользователя
        /// </summary>
        /// <param name="userName">Имя подключаемого пользователя</param>
        public void Connect(string userName)
        {
            // В методе Connect мы сначала получаем id текущего пользователя, 
            // который и обратился к методу Connect, через объект Context.ConnectionId
            // Этот id задается средой и хранит строковое значение (не числовое). 
            //
            var id = Context.ConnectionId;

            // Затем вызываем методы на клиенте через объект Clients.
            //
            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, Name = userName });

                // Посылаем сообщение текущему пользователю
                Clients.Caller.onConnected(id, userName, Users);

                // Посылаем сообщение всем пользователям, кроме текущего
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        /// <summary>
        /// Переопределяем метод OnDisconnected, который выполняет отключение текущего клиента в асинхронном режиме.
        /// Например, при закрытии вкладки браузера клиент по сути выходит из приложения и вызывается метод 
        /// OnDisconnected для текущего клиента.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;

                // Вызов метода на всех клиентах: 
                //
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}