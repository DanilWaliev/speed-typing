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
let counter;
// Указатель на последний введённый пользователем спан
let textNode;
// Текст для печати
let textLow = 'The quick brown fox jumps over the lazy dog. Practice typing daily to improve your speed.';
let textMedium = 'Typing speed is measured in words per minute (WPM). To achieve 60+ WPM, focus on accuracy first, then gradually increase your pace. Avoid looking at the keyboard!';
let textHigh = 'Mastering touch typing requires consistent practice. Place your fingers on the home row (ASDF-JKL;) and use all ten fingers. The more you type without errors, the higher your WPM score will be. Remember: speed comes naturally after accuracy. Ready? Lets begin this timed test now! ';
let text;
const rangeSlider = document.getElementById('range');

// Обработка нажатия кнопки Start
function startInput(event) {
    const rangePosition = rangeSlider.value
    if (rangePosition == 1)
        text = textLow;
    else if (rangePosition == 2)
        text = textMedium;
    else if (rangePosition == 3)
        text = textHigh;

    mainDiv.innerHTML = '';

    for (const char of text) {
        mainDiv.appendChild(getColorCharSpan(char));
    }

    // Получаем первый символ
    textNode = mainDiv.firstChild;
    counter = 0;

    window.addEventListener("keydown", handleKeydown);
    startToRestart(e);
}

// Меняет текст кнопки старт на рестарт и меняет обработчик событий
function startToRestart(e)
{
    e.textContent = "Restart";
    e.removeEventListener("click", startInput);
    e.addEventListener("click", restartInput);
}

// Рестарт ввода символов
function restartInput()
{
    cleanMainDiv();
    startInput();
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
    let resultSpan = document.createElement('p');
    resultSpan.textContent = `${result} letters per sec`;

    resultDiv.appendChild(resultSpan);
}

// Очищения окна вывода от символов
function cleanMainDiv() {
    while (mainDiv.firstChild) {
        mainDiv.removeChild(mainDiv.firstChild)
    }
}

