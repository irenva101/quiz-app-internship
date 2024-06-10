import { Component } from '@angular/core';
import { FormGroup, Validators, FormArray, FormControl, UntypedFormGroup, ValidationErrors } from '@angular/forms';
import { AnswerItem, Category, CreateOrUpdateQuestionCommand, QuestionsClient, TypeButton } from '../api/api-reference';
import { ActivatedRoute, Router } from '@angular/router';
import { FORM_FIELD_ERROR_KEY, setServerSideValidationErrors } from '../validation';



@Component({
  selector: 'app-question-edit',
  templateUrl: './question-edit.component.html'
})
export class QuestionEditComponent {

  TypeButton = TypeButton;
  Category = Category;

  questionForm = new FormGroup({
    type: new FormControl<TypeButton>(TypeButton.Radiobutton, { validators: Validators.required, nonNullable: true }),
    category: new FormControl<Category>(Category.OOP, { validators: Validators.required, nonNullable: true }),
    text: new FormControl<string>('', { validators: Validators.required, nonNullable: true }),
    answers: new FormArray<UntypedFormGroup>([])
  })
  typeOptions = Object.keys(TypeButton).filter(k => !isNaN(Number(k)));
  categoryOptions = Object.keys(Category).filter(k => !isNaN(Number(k)));

  questionId: number | null = null;

  constructor(
    private questionsClient: QuestionsClient,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.questionId = +id;
        this.loadQuestion(this.questionId);
      }
    });
  }

  loadQuestion(id: number): void {
    this.questionsClient.getById(id).subscribe(response => {
      this.questionForm.patchValue({
        type: response.typeButton,
        category: response.category,
        text: response.text
      });
      this.setAnswers(response.answers ?? []);
    });
  }

  setAnswers(answers: AnswerItem[]): void {
    answers.forEach(answer => {
      this.answers.push(this.createAnswer(answer));
    });
  }

  createAnswer(answer: AnswerItem | undefined): FormGroup {
    return new FormGroup({
      id: new FormControl<number | undefined>(answer?.id ?? undefined),
      text: new FormControl<string>(answer?.text ?? '', { validators: [Validators.required], nonNullable: true }),
      isCorrect: new FormControl<boolean>(answer?.isCorrect ?? false, { nonNullable: true })
    });
  }

  get answers(): FormArray {
    return this.questionForm.get('answers') as FormArray;
  }

  addAnswer(): void {
    const answersArray = this.questionForm.get('answers') as FormArray;
    answersArray.push(this.createAnswer(undefined));
  }

  removeAnswer(index: number): void {
    this.answers.removeAt(index);
  }

  onSubmit(): void {
    if (this.questionForm.valid) {

      const formData = this.questionForm.value;

      const command = new CreateOrUpdateQuestionCommand({
        id: undefined,
        category: +formData.category!,
        typeButton: +formData.type!,
        text: formData.text,
        answers: formData.answers!.map(answer => new AnswerItem({
          id: answer.id,
          text: answer.text,
          isCorrect: answer.isCorrect
        }))
      });

      if (this.questionId) {
        command.id = this.questionId;
      }

      this.questionsClient.createOrUpdate(command).subscribe({
        next: _ => this.router.navigate(['questions']),
        error: errors => setServerSideValidationErrors(errors, this.questionForm)
      });

    }
  }

  errorForControl(controlName: string): ValidationErrors | null{
    const control = this.questionForm.get(controlName);
    if(control == null || control.errors == null || control.errors[FORM_FIELD_ERROR_KEY] == null){
      return null;
    }
    return this.questionForm.get(controlName)!.errors![FORM_FIELD_ERROR_KEY];
  }
}
