import { Word } from './models/Word';
import { Injectable } from '@angular/core';

@Injectable()
export class DictionaryService {
  words = [
    { En: 'test', Ru: 'тест' }
  ];
  constructor() { }

  getDictionary(): Word[] {
    return this.words;
  }
}