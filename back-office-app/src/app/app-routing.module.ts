import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuestionEditComponent } from './question-edit/question-edit.component';
import { CreateOrUpdateQuestionCommand } from './api/api-reference';
import { authGuard } from './auth.guard';
import { QuestionsTableComponent } from './questions-table/questions-table.component';

const routes: Routes = [
  { path: 'questions/create', component: QuestionEditComponent, canActivate: [authGuard] },
  { path: 'questions/edit/:id', component: QuestionEditComponent, canActivate: [authGuard] },
  { path: 'questions', component: QuestionsTableComponent, canActivate: [authGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
