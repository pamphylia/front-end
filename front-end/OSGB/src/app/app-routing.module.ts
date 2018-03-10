import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './components/app.component';

const routes: Routes = [
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  { path: 'login', loadChildren: './modules/authModule/auth.module#AuthModule' },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}