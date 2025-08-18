package mysql

import (
	"database/sql"
)

type WordModel struct {
	DB *sql.DB
}

func (m *WordModel) GetWordsByDifficulty(difficulty float64) ([]string, error) {
	// максимальное отклонение от заданного значения
	delta := 1.5
	//tableName := "shrek"
	lowerBound := difficulty - delta
	upperBound := difficulty + delta

	stmt := `SELECT Word, Difficulty FROM shrek WHERE Difficulty > ? AND Difficulty < ?`

	rows, err := m.DB.Query(stmt, lowerBound, upperBound)
	if err != nil {
		return nil, err
	}

	defer rows.Close()

	var words []string

	for rows.Next() {
		w := ""
		e := 0.0

		err := rows.Scan(&w, &e)
		if err != nil {
			return nil, err
		}

		words = append(words, w)

		if err := rows.Err(); err != nil {
			return nil, err
		}
	}

	return words, err
}
