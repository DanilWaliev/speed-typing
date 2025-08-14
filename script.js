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
// для графика
let resultChart = null;
let startTime, endTime;
// массив с результатами
let resultArray = [];
// кол-во попыток(зависимость графика по абициссе)
let countArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
// Текст для печати
let textLow = 'the sun shines bright in the clear blue sky birds fly high above the green trees';
let textMedium = 'typing quickly and accurately is a skill that takes time to develop regular practice helps build muscle memory and improves your speed and stay focused and keep your hands steady';
let textHigh = 'to become a fast typist you must train your fingers to move without thinking and start with simple exercises and gradually move to complex texts avoid looking at the keyboard and trust your muscle memory speed will come naturally with patience and persistence remember even experts were once beginners who never gave up';
let text;
// Ползунок
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
   endTime = new Date().getTime();
    window.removeEventListener('keydown', handleKeydown);
    
    // Рассчитываем скорость
    const timeInSeconds = (endTime - startTime) / 1000;
    result = (text.length / timeInSeconds).toFixed(1);
    resultArray.push(parseFloat(result));
    
    printResult();
    // Добавление данных графику
    updateChart();
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
    const stopButtonStart = document.querySelector(".buttonStart");
    stopButtonStart.addEventListener("keydown", function (e){
        if (e.key === " ") 
            e.preventDefault();
    });

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

        if (textNode === null){
             endInput();
        }
    }
}

// Завершение ввода
function endInput() {
    endTime = new Date().getTime();
    window.removeEventListener('keydown', handleKeydown);
    
    const timeInSeconds = (endTime - startTime) / 1000;
    result = (text.length / timeInSeconds).toFixed(1);
    resultArray.push(parseFloat(result));
    
    printResult();
    // Добавление данных графику
    updateChart();
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


// График
function updateChart() {
    const ctx = document.querySelector('.graph');
    
    if (!resultChart) {
        //график при первом запуске(пустой)
        resultChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: countArray,
                datasets: [{
                    label: 'Graph',
                    data: Array(countArray.length).fill(null),
                    borderColor: 'rgb(75, 192, 192)',
                    borderWidth: 3,
                    tension: 0.1,
                    pointBackgroundColor: 'rgb(75, 192, 192)',
                    pointRadius: 5
                }]
            },
            options: {
                responsive: true,
                color: '#D3D3D3',
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        top: 20,
                        right: 20,
                        bottom: 20,
                        left: 20
                    }
                },
                lugins: {
                    legend: {
                        labels: {
                            font: {
                                family: "monospace",
                                size: 20,
                                weight: 'normal'
                            },
                            color: '#D3D3D3'
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        grid: {
                            color: '#D3D3D3',
                            lineWidth: 1
                        },
                        ticks: {
                            color: '#FFFFFF', 
                            font: {
                                size: 18,    
                                weight: 'normal' 
                            }
                        },
                        title: {
                            color: '#D3D3D3',
                            display: false,
                            text: 'Speed letters per sec',
                            family: "monospace",
                            size: 18
                        }
                    },
                    x: {
                        grid: {
                            color: '#D3D3D3',
                            lineWidth: 1
                        },
                        ticks: {
                            color: '#FFFFFF', 
                            font: {
                                size: 18,
                                weight: 'both'     
                            }
                        },
                        title: {
                            color: '#D3D3D3',
                            display: false,
                            text: 'Attempt',
                            family: "monospace",
                            size: 18
                        },
                        ticks: {
                            stepSize: 1,
                            color: '#D3D3D3',
                        }
                    }
                }
            }
        });
    }
    
    // Обновление данных графика
    const chartData = countArray.map((_, i) => resultArray[i] || null);
    resultChart.data.datasets[0].data = chartData;
    resultChart.update();
}
