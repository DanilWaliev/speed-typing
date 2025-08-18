package main

import (
	"math/rand"
	"net/http"
	"strconv"
	"strings"
	"time"
)

func (app *application) home(w http.ResponseWriter, r *http.Request) {
	if r.URL.Path != "/" {
		app.notFound(w)
	}

	app.render(w)
}

func (app *application) sendWords(w http.ResponseWriter, r *http.Request) {
	difficulty, err := strconv.ParseFloat(r.URL.Query().Get("difficulty"), 64)
	if err != nil {
		app.serverError(w, err)
	}

	words, err := app.words.GetWordsByDifficulty(difficulty)
	if err != nil {
		app.serverError(w, err)
	}

	// перешивание порядка слов
	rnd := rand.New(rand.NewSource(time.Now().UnixNano()))
	rnd.Shuffle(len(words), func(i, j int) {
		words[i], words[j] = words[j], words[i]
	})

	text := strings.Join(words, " ")

	w.Write([]byte(text))
}
