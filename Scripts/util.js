/*
Весь код на стороне клиента находится в функции jQuery,
за ее пределами определены две функции - 
htmlEncode (для кодирования тегов, чтобы пресечь возможные попытки вставок кода javascript и прочие нехорошести) 
и AddUser (для добавления данных пользователя в список).
*/

$(function () {
    $('#chatBody').hide();
    $('#loginBlock').show();

    // Чтобы взаимодействовать с хабом, получаем прокси хаба:
    //
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;

    // Выше в хабе в коде C# у нас было определено обращение к функциям клиента: Clients.All.addMessage(name, message);. 
    // Функция addMessage - это и есть функция, определенная для chat.client.addMessage. 
    // Подобным образом мы можем обращаться на сервере и к другим функциям клиента.
    //
    chat.client.addMessage = function (name, message) {
        // Добавление сообщений на веб-страницу 
        $('#chatroom').append('<p><b>' + htmlEncode(name)
        + '</b>: <i>' + htmlEncode(message) + '</i></p>');
    };

    // Функция, вызываемая при подключении нового пользователя
    //
    chat.client.onConnected = function (id, userName, allUsers) {
        $('#loginBlock').hide();
        $('#chatBody').show();

        // установка в скрытых полях имени и id текущего пользователя
        $('#hdId').val(id);
        $('#username').val(userName);

        $('#header').html('<p>Добро пожаловать, <b>' + userName + '</b> (id = <span style="color:blue;">' + id + '</span>)</p>');

        // Добавление всех пользователей
        for (i = 0; i < allUsers.length; i++) {
            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
    }

    // Добавляем нового пользователя
    //
    chat.client.onNewUserConnected = function (id, name) {
        AddUser(id, name);
    }

    // Удаляем пользователя
    //
    chat.client.onUserDisconnected = function (id, userName) {
        $('#' + id).remove();
    }

    // Открываем соединение
    //
    // Для открытия подключения мы вызываем метод $.connection.hub.start().done(), передавая в него функцию.
    // В этой функции мы вешаем обработчики кнопок, по нажатию на которые происходит обращение на сервер.
    //
    $.connection.hub.start().done(function () {

        // Выражение chat.server представляет собой обращение к методам хаба на сервере.
        //
        $('#sendmessage').click(function () {

            // Вызываем у хаба метод Send
            chat.server.send($('#username').val(), $('#message').val());
            $('#message').val('');
        });

        // обработка логина
        $("#btnLogin").click(function () {
            var name = $("#txtUserName").val();

            if (name.length > 0) {

                // Вызываем у хаба метод Connect
                chat.server.connect(name);
            }
            else {
                alert("Введите имя");
            }
        });
    });
});

// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

// Добавление нового пользователя
function AddUser(id, name) {
    var userId = $('#hdId').val();

    //if (userId != id) {
    //    $("#chatusers").append('<p id="' + id + '"><b>' + name + '</b></p>');
    //}

    $("#chatusers").append('<p id="' + id + '"><b>' + name + '</b> (id = <span style="color:blue;">' + id + '</span>)</p>');

}