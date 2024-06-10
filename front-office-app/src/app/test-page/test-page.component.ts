import { Component, OnInit } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';
import { GenerateExamCommand, GenerateExamResponse, QuestionsClient, CandidateClient } from '../api/api-reference';

@Component({
  selector: 'app-test-page',
  templateUrl: './test-page.component.html',
  styleUrl: './test-page.component.css'
})
export class TestPageComponent implements OnInit {
  questions: any = [];
  currentQuestionIndex: number = 0;
  currentQuestion: any = null;
  selectedAnswer: string = '';
  selectedAnswers: { [key: number]: any } = {};
  errorMessage: string = '';
  candidateId: number | undefined;

  constructor(
    private route: ActivatedRoute,
    private questionsClient: QuestionsClient,
  ) { }

  ngOnInit(): void {
this.route.queryParams.subscribe(params=>{
  this.candidateId = params['candidateId'] ? +params['candidateId'] : undefined;
});
  }
  nextQuestion() {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.saveCurrentAnswer();
      this.currentQuestionIndex++;
      this.currentQuestion = this.questions[this.currentQuestionIndex];
      this.loadCurrentAnswer();
    }
  }

  previousQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.saveCurrentAnswer();
      this.currentQuestionIndex--;
      this.currentQuestion = this.questions[this.currentQuestionIndex];
      this.loadCurrentAnswer();
    }
  }
  saveCurrentAnswer() {
    if (this.currentQuestion.type === 1) {
      this.selectedAnswers[this.currentQuestion.id] = this.selectedAnswer;
    } else {
      this.selectedAnswers[this.currentQuestion.id] = { ...this.selectedAnswers[this.currentQuestion.id] };
    }
  }
  loadCurrentAnswer() {
    const savedAnswer = this.selectedAnswers[this.currentQuestion.id];
    if (this.currentQuestion.type === 1) {
      this.selectedAnswer = savedAnswer || '';
    }
  }

  startTest() {
    const command=new GenerateExamCommand();
    command.candidateId=18;
    this.questionsClient.generateExam(command).subscribe({
      next: response => {
        this.questions = response.questions;
        if (this.questions.length > 0) {
          this.currentQuestion = this.questions[0];
        }
      },
      error: (error) => {
        if(error.error&& error.error.message ==='Candidate has already taken an exam.'){
          this.errorMessage = 'You have already taken the exam.';
        }else{
        console.error('Error fetching random questions:', error);
        this.errorMessage = 'You have already taken the exam.';
      }
      }
    });
  }
}