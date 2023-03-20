# Csv Lexing
Hier doe ik een poging tot het maken van een simpele csv-lexer die voldoet aan overgrote deel van de specs.

## Features
- [x] comment-support
- [x] full string-support
- [x] negeren van trailing space-chars e.g "John&emsp;,&emsp;Doe" ->  "John" en "Doe"
- [x] Embedded line-breaks middels double-qoutes
- [x] splitten van de waardes
- [x] Asynchronous