package main

// задачи по проекту
// ниже будут отображены задачи, относящиеся к проекту в целом, а не к конкретной его части
// # реализовать загрузку главной страницы и статических файлов
// # реализовать загрузку слов на главную страницу

import (
	"database/sql"
	"flag"
	"log"
	"net/http"
	"os"
	"speed-typing/pkg/models/mysql"

	_ "github.com/go-sql-driver/mysql"
)

// структура для необходимых зависомостей
type application struct {
	errorLog *log.Logger
	infoLog  *log.Logger
	words    *mysql.WordModel
}

type secretData struct {
	dsn string
}

func main() {
	// парсинг флагов с командной строки
	addr := flag.String("addr", ":4000", "HTTP Network address")
	dsn := flag.String("dsn", "web:pass@/snippetbox?parseTime=true", "Название MySQL источника данных")
	flag.Parse()

	// создание логеров
	infoLog := log.New(os.Stdout, "INFO\t", log.Ldate|log.Ltime)
	errorLog := log.New(os.Stderr, "ERROR\t", log.Ldate|log.Ltime|log.Lshortfile)

	// создание пула подключений к БД
	db, err := openDB(*dsn)
	if err != nil {
		errorLog.Fatal(err)
	}
	defer db.Close()

	// инициализация структуры приложения с необходимыми зависимостями
	app := &application{
		errorLog: errorLog,
		infoLog:  infoLog,
		words:    &mysql.WordModel{DB: db},
	}

	// инициализация структуры http.Server
	srv := &http.Server{
		Addr:     *addr,
		ErrorLog: app.errorLog,
		Handler:  app.routes(),
	}

	// запуск сервера
	infoLog.Printf("Запуск сервера на %s", *addr)
	err = srv.ListenAndServe()
	errorLog.Fatal(err)

}

func openDB(dsn string) (*sql.DB, error) {
	db, err := sql.Open("mysql", dsn)
	if err != nil {
		return nil, err
	}
	if err = db.Ping(); err != nil {
		return nil, err
	}
	return db, nil
}
