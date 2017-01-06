import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http'
import { BrowserModule } from '@angular/platform-browser'
import { RouterModule } from '@angular/router'

import { ROUTING } from './app.routing';
import { App } from './app/app.component';
import { Home } from './app/components/home/home.component';
import { DictionaryComponent } from './app/components/dictionary/dictionary.component';
import { DictionaryService } from './app/dictionary.service';

@NgModule({
  imports: [BrowserModule, HttpModule, ROUTING],
  declarations: [App, Home, DictionaryComponent],
  providers: [DictionaryService],
  bootstrap: [App]
})
export class AppModule { }
