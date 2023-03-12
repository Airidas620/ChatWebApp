"use strict";
//TODO move repetitive code

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var chatName;
var messageInput;

var currentUser;
var selectedUser = null;
var isSelectionAnUser = true;

var chatHistory = {}
var groupChatHistory = {}

class MessageDetails {
    constructor(sender, message) {
        this.sender = sender;
        this.message = message;
    }
}

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

//Start a connection
connection.start().then(function () {

    document.getElementById("sendButton").disabled = false;

    chatName = document.getElementById("chatName");
    messageInput = document.getElementById("messageInput");

    document.getElementById("messageInput").addEventListener("keypress", function (event) {

        if (event.key === "Enter") {

            event.preventDefault();

            document.getElementById("sendButton").click();
        }
    });

    document.getElementById("groupInput").addEventListener("keypress", function (event) {

        if (event.key === "Enter") {

            event.preventDefault();

            document.getElementById("joinGroup").click();
        }
    });

    document.getElementById("userList").addEventListener("click", function (e) {
        if (e.target && e.target.matches(".list-item")) {

            RemoveSelectionClass(selectedUser);

            selectedUser = e.target.id;

            const selectedListItem = $('#userList').find('#' + selectedUser);

            if (selectedListItem.hasClass("list-item-unread")) {
                selectedListItem.toggleClass("list-item-unread");
            }

            if (!selectedListItem.hasClass("list-item-selected")) {
                selectedListItem.toggleClass("list-item-selected");
            }

            isSelectionAnUser = true;

            chatName.innerText = e.target.id;

            selectedUser = e.target.id;

            document.getElementById("messagesList").innerHTML = '';

            if (chatHistory.hasOwnProperty(selectedUser)) {
                chatHistory[selectedUser].forEach(messageDetail => {

                    AppendMessageToTheList(messageDetail.sender, messageDetail.message);

                });
            }
        }
    });

    document.getElementById("groupList").addEventListener("click", function (e) {
        if (e.target && e.target.matches(".list-item")) {

            RemoveSelectionClass(selectedUser);

            selectedUser = e.target.id;

            const selectedListItem = $('#groupList').find('#' + selectedUser);

            if (selectedListItem.hasClass("list-item-unread")) {
                selectedListItem.toggleClass("list-item-unread");
            }

            if (!selectedListItem.hasClass("list-item-selected")) {
                selectedListItem.toggleClass("list-item-selected");
            }

            isSelectionAnUser = false;

            chatName.innerText = e.target.id;

            document.getElementById("messagesList").innerHTML = '';

            if (groupChatHistory.hasOwnProperty(selectedUser)) {
                groupChatHistory[selectedUser].forEach(messageDetail => {

                    AppendMessageToTheList(messageDetail.sender, messageDetail.message);

                });
            }

        }
    });

}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("GetCurrentOnlineUsers", function (users) {
    users.forEach(user => {
        AppendUserToOnlineList(user);
    });
});

connection.on("GetUserName", function (user) {
    currentUser = user;
});

connection.on("UserWentOnline", function (user) {
    AppendUserToOnlineList(user);
});

connection.on("UserWentOffline", function (user) {
    if (selectedUser == user) {
        AppendMessageToTheList("Information:", "User " +  user + " is offline");
    }
    $('#userList').find('#' + user).remove();
    //document.getElementById("userList").removeChild(document.getElementById(user));
});

connection.on("GetYourGroups", function (groups) {
    console.log(groups);
    groups.forEach(group => {
        AppendGroupToGroupList(group);
    });
});

connection.on("JoinAGroup", function (group) {
    AppendGroupToGroupList(group);
});

connection.on("ReceiveAGroupMessage", function (groupName, sender, message) {

    if (selectedUser != groupName) {
        if (!$('#groupList').find('#' + groupName).hasClass("list-item-unread")) {
            $('#groupList').find('#' + groupName).toggleClass("list-item-unread");
        }
    }

    const messageDetails = new MessageDetails(sender, message);

    if (groupName in groupChatHistory) {
        groupChatHistory[groupName].push(messageDetails);
    }
    else {
        groupChatHistory[groupName] = [messageDetails];
    }

    if (groupName == selectedUser) {
        AppendMessageToTheList(sender, message);
    }
});


connection.on("ReceiveDirectMessage", function (sender, message) {

    if (selectedUser != sender) {
        if (!$('#userList').find('#' + sender).hasClass("list-item-unread")) {
            $('#userList').find('#' + sender).toggleClass("list-item-unread");
            console.log("asd");
        }
    }

    const messageDetails = new MessageDetails(sender, message);

    if (sender in chatHistory) {
        chatHistory[sender].push(messageDetails);
    }
    else {
        chatHistory[sender] = [messageDetails];
    }

    if (sender == selectedUser) {
        AppendMessageToTheList(sender, message);
    }
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    var message = messageInput.value;

    if (selectedUser != null && message !== "") {
        document.getElementById("messageInput").value = "";

        if (isSelectionAnUser) {

            connection.invoke("SendDirectMessage", currentUser, selectedUser, message).catch(function (err) {
                return console.error(err.toString());
            });


            const messageDetails = new MessageDetails(currentUser, message);

            if (selectedUser in chatHistory) {
                chatHistory[selectedUser].push(messageDetails);
            }
            else {
                chatHistory[selectedUser] = [messageDetails];
            }

            AppendMessageToTheList(currentUser, message);
        }
        else {
            connection.invoke("SendAGroupMessage", selectedUser, currentUser, message).catch(function (err) {
                return console.error(err.toString());
            });

            const messageDetails = new MessageDetails(currentUser, message);

            if (selectedUser in groupChatHistory) {
                groupChatHistory[selectedUser].push(messageDetails);
            }
            else {
                groupChatHistory[selectedUser] = [messageDetails];
            }

            AppendMessageToTheList(currentUser, message);
        }
    }

    event.preventDefault();
});

document.getElementById("joinGroup").addEventListener("click", function (event) {
    var groupName = document.getElementById("groupInput").value;

    if (groupName) {
        connection.invoke("JoinOrCreateAGroup", groupName).catch(function (err) {
            return console.error(err.toString());
        });
    }

    event.preventDefault();
});



function AppendUserToOnlineList(user) {
    $('#userList').append($('<li>' + user + '</li>').attr('id', user).attr('class', 'list-item'));
}

function AppendGroupToGroupList(group) {
    $('#groupList').append($('<li>'+group+'</li>').attr('id', group).attr('class', 'list-item'));
}

function AppendMessageToTheList(sender, message) {
    $("#messagesList").append('<li class="list-message">' + sender + '<br "> ' + message + ' </br > </li>');
}

function RemoveSelectionClass(selection) {

    if (isSelectionAnUser) {
        $('#userList').find('#' + selection).toggleClass("list-item-selected");
        return;
    }

    $('#groupList').find('#' + selection).toggleClass("list-item-selected");
}