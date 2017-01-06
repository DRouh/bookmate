import { Word } from '../../models/Word';
import { DictionaryService } from '../../dictionary.service';
import { Component, OnInit } from '@angular/core';

@Component({
  moduleId: 'module.id',
  selector: 'dictionary',
  template: `
  <div>
  <h2>Dictionary</h2>
  <table border="1|0">
    <caption>English-Russian dictionary</caption>
    <thead>
      <tr>
        <th>English</th>
        <th>Russian</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let word of words">
        <td>{{word.En}}</td>
        <td>{{word.Ru}}</td>
      </tr>
      </tbody>
  </table>
</div>
  `
  //templateUrl: 'dictionary.component.html'
})
export class DictionaryComponent implements OnInit {
  words: Word[] = [];
  constructor(private dictionaryService: DictionaryService) { }

  ngOnInit(): void {
    this.words = this.dictionaryService.getDictionary();
  }
}

