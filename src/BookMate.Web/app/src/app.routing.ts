import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Home } from './app/components/home/home.component';
import { DictionaryComponent } from './app/components/dictionary/dictionary.component';

export const ROUTES: Routes = [
  { path: '', component: Home },
  { path: 'dictionary', component: DictionaryComponent }
];

export const ROUTING: ModuleWithProviders = RouterModule.forRoot(ROUTES);
