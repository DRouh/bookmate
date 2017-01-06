import { Component } from '@angular/core';
import { Home } from './components/home/home.component';
import { DictionaryComponent } from './components/dictionary/dictionary.component';

@Component({
  moduleId: 'module.id',
  selector: 'my-app',
  template: `
  <h1>Book Mate</h1>
<div>
  <a [routerLink]="['/']">Home</a>
  <a [routerLink]="['/dictionary']">Dictionary</a>
</div>
<div>
  <router-outlet></router-outlet>
</div>
  `
  //templateUrl: './app/app.component.html'
})
export class App {}
