import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestPageComponent } from './test-page/test-page.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  {path: '', component:RegisterComponent},
  {path: 'test-page', component:TestPageComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
