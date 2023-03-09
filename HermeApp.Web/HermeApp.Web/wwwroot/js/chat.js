"use strict";
//TODO move repetitive code

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var currentUser;
var selectedUser = null;
var isSelectionAnUser = true;

var chatHistory = {}
var groupChatHistory = {}

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

//Start a connection
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    document.getElementById("userList").addEventListener("click", function (e) {
        if (e.target && e.target.matches("li.user")) {
            isSelectionAnUser = true;

            selectedUser = e.target.id;
            document.getElementById("Testing").innerHTML = "you've pressed";

            document.getElementById("messagesList").innerHTML = '';

            if (chatHistory.hasOwnProperty(selectedUser)) {
                chatHistory[selectedUser].forEach(message => {
                    //SaveText()

                    var li = document.createElement("li");
                    li.className = 'text';

                    document.getElementById("messagesList").appendChild(li);

                    li.textContent = `${selectedUser}:${message}`;
                });
            }
        }
    });

    document.getElementById("groupList").addEventListener("click", function (e) {
        if (e.target && e.target.matches("li.group")) {

            isSelectionAnUser = false;

            console.log(e.target.id)
            selectedUser = e.target.id;
            document.getElementById("Testing").innerHTML = "you've pressed group";

            document.getElementById("messagesList").innerHTML = '';

            if (groupChatHistory.hasOwnProperty(selectedUser)) {
                groupChatHistory[selectedUser].forEach(message => {
                    var li = document.createElement("li");
                    li.className = 'text';

                    document.getElementById("messagesList").appendChild(li);

                    li.textContent = `${selectedUser}:${message}`;
                });
            }

        }
    });

}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("GetCurrentOnlineUsers", function (users) {
    users.forEach(user => {
        var li = document.createElement("li");
        li.className = 'user';
        li.id = user;

        document.getElementById("userList").appendChild(li);
        li.textContent = `${user}`;
    });
});

connection.on("UserWentOnline", function (user) {
    var li = document.createElement("li");
    li.className = 'user';
    li.id = user;

    document.getElementById("userList").appendChild(li);
    li.textContent = `${user}`;
});

connection.on("UserWentOffline", function (user) {
    document.getElementById("userList").removeChild(document.getElementById(user));
});

///////////////////////
document.getElementById("joinGroup").addEventListener("click", function (event) {
    var groupName = document.getElementById("groupInput").value;

    //if (selectedUser != null) {
        connection.invoke("JoinOrCreateAGroup", groupName).catch(function (err) {
            return console.error(err.toString());
        });
    //}

    event.preventDefault();

    console.log("Ended");
});

connection.on("JoinAGroup", function (groupName) {
    console.log("hi");
    var li = document.createElement("li");
    li.className = 'group';
    li.id = groupName;

    document.getElementById("groupList").appendChild(li);
    li.textContent = `${groupName}`;
});

connection.on("ReceiveAGroupMessage", function (groupName, message) {
    if (groupName in groupChatHistory) {
        groupChatHistory[groupName].push(message);
    }
    else {
        groupChatHistory[groupName] = [message];
    }
    console.log(selectedUser);
    console.log(groupName);
    console.log(groupName == selectedUser);


    if (groupName == selectedUser) {
        SaveText("messagesList", `${selectedUser}: ${message}`);
    }
});

connection.on("GetYourGroups", function (groups) {

    console.log(groups);
    groups.forEach(group => {
        var li = document.createElement("li");
        li.className = 'group';
        li.id = group;

        document.getElementById("groupList").appendChild(li);
        li.textContent = `${group}`;
    });
});

///////////////////////

connection.on("ReceiveDirectMessage", function (userFrom, userTo, message) {
    console.log("hi3");

    if (userFrom in chatHistory) {
        chatHistory[userFrom].push(message);
    }
    else {
        chatHistory[userFrom] = [message];
    }

    if (userFrom == selectedUser) {
        SaveText("messagesList", `${userFrom}: ${ userTo }: ${ message }`);
    }
});



document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    console.log(currentUser);

    if (selectedUser != null) {
        if (isSelectionAnUser) {
            connection.invoke("SendDirectMessage", currentUser, selectedUser, message).catch(function (err) {
                return console.error(err.toString());
            });

            //non error proof
            if (selectedUser in chatHistory) {
                chatHistory[selectedUser].push(message);
            }
            else {
                chatHistory[selectedUser] = [message];
            }

            SaveText("messagesList", message);

            /*var li = document.createElement("li");
            li.className = 'text';
    
            document.getElementById("messagesList").appendChild(li);
    
            li.textContent = `${selectedUser}:${message}`;*/
        }
        else {
            connection.invoke("SendAGroupMessage", selectedUser, message).catch(function (err) {
                return console.error(err.toString());
            });

            //non error proof
            if (selectedUser in chatHistory) {
                chatHistory[selectedUser].push(message);
            }
            else {
                chatHistory[selectedUser] = [message];
            }

            SaveText("messagesList", message);
        }
    }

    event.preventDefault();
});

function SaveText(lobby, message) {
    var li = document.createElement("li");
    li.className = 'text';

    document.getElementById(lobby).appendChild(li);

    li.textContent = message;

    //li.textContent = user + ": " + message;

}
