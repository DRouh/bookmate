import { Word } from '../../models/Word';
import { DictionaryService } from '../../dictionary.service';
import { Component, OnInit } from '@angular/core';

@Component({
  moduleId: 'module.id',
  selector: 'dictionary',
  templateUrl: './dictionary.component.html'
})
export class DictionaryComponent implements OnInit {
  words: Word[] = [];
  constructor(private dictionaryService: DictionaryService) { }

  ngOnInit(): void {
    this.words = this.dictionaryService.getDictionary();
  }
}

