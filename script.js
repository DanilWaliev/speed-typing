/*
Скрипт отвечает за запуск, обработку и окончание ввода пользователем текста

1. В блок с id="mainDiv" помещаются спаны (span), содержащие одну букву.
(т.е. каждая буква - это отдельный спан). 
2. Запускается обработка нажатия клавиши пользователем.
3. Когда пользователь введёт последний символ, обработка нажатий выключается.
*/

// Получаем div, в котором отображаются буквы
let mainDiv = document.getElementById("inputDiv");

let resultDiv = document.getElementById("resultDiv");
// Счетчик введённых пользователем символов
let counter = 0;
// Указатель на последний введённый пользователем спан
let textNode;
// Текст для печати
let text;

// Обработка нажатия кнопки Start
function startInput(e) {
    text = 'just go on and faith will soon return';
    for (char of text) {
        mainDiv.appendChild(getColorCharSpan(char));
    }

    // Получаем первый символ в div'е (первый элемент в div'е почему то "text", поэтому получаем следующий после первого)
    textNode = mainDiv.firstChild;

    window.addEventListener("keydown", handleKeydown);
}

// Проверка клавиши (true - буквы, пробел или BackSpace; false - остальные символы)
function isValidKey(key) {
    let charCode = key.charCodeAt(0);
    return (charCode === 32 || charCode === 66 || (charCode >= 97 && charCode <= 122));
}

// "Конструктор" спана
function getColorCharSpan(char) {
    const span = document.createElement('span');
    span.textContent = char;
    span.classList.add('unwrittenSpan');
    return span;
}

// Задает переданному спану закругленные края
// isForward - направление ввода пользователя (если вводит символ - true, если стирает - false)
function setRoundEdges(span, isForward) {
    span.classList.remove('singleSpan');
    span.classList.remove('firstSpan');
    span.classList.remove('lastSpan');

    if (counter === 1) {
        if (isForward) {
            span.classList.add('singleSpan');
        }
        else {
            span.previousElementSibling.classList.remove('firstSpan');
            span.previousElementSibling.classList.add('singleSpan');
        }
    }
    else if (counter === 2) {
        if (isForward) {
            span.previousElementSibling.classList.remove('singleSpan');
            span.previousElementSibling.classList.add('firstSpan');
            span.classList.add('lastSpan');
        }
        else {
            span.previousElementSibling.classList.add('lastSpan');
        }
    }
    else {
        if (isForward) {
            span.previousElementSibling.classList.remove('lastSpan');
            span.classList.add('lastSpan');
        }
        else {
            span.previousElementSibling.classList.add('lastSpan');
        }

    }
}

// Обработка нажатия клавиши
function handleKeydown(e) {
    if (!isValidKey(e.key)) return;

    if (counter === 0) startTime = new Date().getTime();

    // Глушим стандартное действие на пробел
    if (e.keyCode === 32 && e.target === document.body) {  
        e.preventDefault();  
    }  

    if (e.key === 'Backspace') {
        counter--;

        textNode.previousElementSibling.classList.remove('writtenCorrectSpan');
        textNode.previousElementSibling.classList.remove('writtenWrongSpan');
        textNode.previousElementSibling.classList.add('unwrittenSpan');

        textNode = textNode.previousElementSibling;

        setRoundEdges(textNode);
    }
    else if (textNode.textContent === e.key) {
        counter++;

        textNode.classList.remove('unwrittenSpan');
        textNode.classList.add('writtenCorrectSpan');
        setRoundEdges(textNode, true);

        textNode = textNode.nextElementSibling;

        if (textNode === null) endInput();
    }
    else {
        counter++;
        textNode.classList.remove('unwrittenSpan');
        textNode.classList.add('writtenWrongSpan');
        
        setRoundEdges(textNode, true);

        textNode = textNode.nextElementSibling;

        if (textNode === null) endInput();
    }
}

// Завершение ввода
function endInput() {
    endTime = new Date().getTime();
    window.removeEventListener('keydown', handleKeydown);
    printResult();
}

// Вывод результата в 
function printResult() {
    // приведение к символам/час
    result = (text.length / (endTime - startTime) * 1000).toFixed(1);
    let resultSpan = document.createElement('span');
    resultSpan.textContent = `${result} letters per sec`;

    resultDiv.appendChild(resultSpan);
}

