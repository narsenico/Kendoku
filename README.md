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

Dettagli:
1. applico gli aiuti
2. inizio iterazione
3. per ogni cella applico regole sudoku
4. per ogni cella applico constraint
5. se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
6. reitero da 2 fino a che tutte le celle non sono risolte

## TODO
- [x] Terminare configurazione (da file)
- [ ] Migliorare output (errori e messaggistica in generale)
- [ ] Ottimizzare risoluzione constraint in [SimpleResolverImpl](Implementations/SimpleResolverImpl.cs) che è abominevole
