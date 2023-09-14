# Kendoku

## Regole
- Quelle del Sudoku
- Più altri vincoli dati dalla somma di due o più celle adiacenti

## Caratteristiche
- Ogni cella mantiene i valori possibili che può assumere
- Le celle con un solo valore possibile sono _risolte_

## Procedura
- Per ogni cella si tenta di elminare i valori possibili andando ad applicare i vincoli su riga/colonna (sudoku) e somma tra celle adiacenti (kendoku)
- Si reitera la procedura finché tutte le celle non sono risolte