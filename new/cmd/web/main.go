package main

// задачи по проекту
// ниже будут отображены задачи, относящиеся к проекту в целом, а не к конкретной его части
// # реализовать загрузку главной страницы и статических файлов
// # реализовать загрузку слов на главную страницу

import (
	"flag"
	"log"
	"net/http"
	"os"
)

// структура для необходимых зависомостей
type application struct {
	errorLog *log.Logger
	infoLog  *log.Logger
}

func main() {
	// парсинг флагов с командной строки
	addr := flag.String("addr", ":4000", "HTTP Network address")
	flag.Parse()

	// создание логеров
	infoLog := log.New(os.Stdout, "INFO\t", log.Ldate|log.Ltime)
	errorLog := log.New(os.Stderr, "ERROR\t", log.Ldate|log.Ltime|log.Lshortfile)

	// инициализация структуры приложения с необходимыми зависимостями
	app := &application{
		errorLog: errorLog,
		infoLog:  infoLog,
	}

	// инициализация структуры http.Server
	srv := &http.Server{
		Addr:     *addr,
		ErrorLog: app.errorLog,
		Handler:  app.routes(),
	}

	// запуск сервера
	infoLog.Printf("Запуск сервера на %s", *addr)
	err := srv.ListenAndServe()
	errorLog.Fatal(err)

}
