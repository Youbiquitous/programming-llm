///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

// INTERNAL FUNCTIONs CALLED BY HANDLERS

///////////////////////////////////////////////////////////////////
// 
//  Gets the translation of a given text
//  through the GPT engine  
// 
function __prontoProcessTranslation(button, emailContainer, language) {
    button.spin();
    Ybq.post("/translate",
        {
            email: emailContainer.val(),
            destination: language
        },
        function (data) {
            button.unspin();
            emailContainer.val(data);
        });
}

///////////////////////////////////////////////////////////////////
// 
//  Gets the text of an asked question and processes it 
//  through the GPT engine  
// 
function __prontoProcessQuestion(button, orig, message, chatContainer) {
    button.spin();
    var userMessage = '<div class="row justify-content-end"><div class="card p-3 mb-2 col-md-10 col-lg-7">' + message + '</div></div>';
    chatContainer.append(userMessage);
    var tempId = Date.now();
    var assistantMessage = '<div class="row justify-content-start"><div class="card p-3 mb-2 col-md-10 col-lg-7 bg-mediumgray" id="chat-' + tempId+'"></div></div > ';
    chatContainer.append(assistantMessage);

    const eventSource = new EventSource("/chat/message?orig=" + orig + "&msg=" + message);

    // Add event listener for the 'message' event
    eventSource.onmessage = function (event) {
        // Append the text to the container
        $("#chat-" + tempId)[0].innerHTML += event.data;
    };

    // Add event listeners for errors and close events
    eventSource.onerror = function (event) {
        // Handle SSE errors here
        console.error('SSE error:', event);
        eventSource.close();
        var textContainer = document.getElementById('chat-' + tempId);
        if (textContainer) {
            textContainer.innerHTML += "<button class='btn btn-xs btn-copy bg-white mt-2' onclick='CopyText(this)'><i class='fa fa-paste'></i></button>";
            button.unspin();
        }
    };
}

function __prontoClearConversation() {
    Ybq.post("/chat/clear",
        {
        },
        function (data) {
            window.location.reload();
        });
}