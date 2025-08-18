package models

import "errors"

var errNoRecord = errors.New("models: подходящей записи нет")

type WordDifficulty struct {
	Word       string
	Difficulty float64
}
