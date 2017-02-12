namespace BookMate.Processing

module DomainModel = 

  type Sentence = 
    | Sentence of string 
    | EmptySentence
  type Chapter = 
    | Chapter of seq<Sentence>
    | EmptyChapter

  type Book =
    | Book of seq<Chapter>
    | EmptyBook
  