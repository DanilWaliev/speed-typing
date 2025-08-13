package main

import (
	"fmt"
	"html/template"
	"net/http"
	"runtime/debug"
)

// записывает в лог сообщение об ошибке и отправляет пользователю ответ 500 (Internal server error)
func (app *application) serverError(w http.ResponseWriter, err error) {
	trace := fmt.Sprintf("%s\n%s", err.Error(), debug.Stack())
	app.errorLog.Output(2, trace)

	http.Error(w, http.StatusText(http.StatusInternalServerError), http.StatusInternalServerError)
}

// отправляет пользователю ответ с кодом ошибки
func (app *application) clientError(w http.ResponseWriter, status int) {
	http.Error(w, http.StatusText(status), status)
}

// обертка над clientError для отправки ответа "Не найдено"
func (app *application) notFound(w http.ResponseWriter) {
	app.clientError(w, http.StatusNotFound)
}

func (app *application) render(w http.ResponseWriter) {
	tmpl, err := template.ParseFiles("./ui/html/home.html")

	if err != nil {
		app.serverError(w, err)
		return
	}

	err = tmpl.Execute(w, nil)
	if err != nil {
		app.serverError(w, err)
		return
	}

}
