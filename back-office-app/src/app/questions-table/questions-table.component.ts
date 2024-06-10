import { Component, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { debounceTime } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { GetAllQuestionsResponse, QuestionsClient } from '../api/api-reference';


@Component({
  selector: 'app-questions-table',
  templateUrl: './questions-table.component.html',
  styleUrls: ['./questions-table.component.css']
})
export class QuestionsTableComponent implements OnInit {
  dataSource = new MatTableDataSource<GetAllQuestionsResponse>();
  displayedColumns: string[] = ['type', 'category', 'text', 'actions'];
  pageSizeOptions = [1, 2, 4, 12];
  pageSize = 12;
  pageIndex = 0;
  totalCount: number | undefined;
  currentFilter: string | undefined;
  filterControl = new FormControl('');

  constructor(private questionsClient: QuestionsClient, private router: Router) { }

  ngOnInit(): void {
    this.getQuestionList();

    this.filterControl.valueChanges
      .pipe(debounceTime(500))
      .subscribe(filterValue => {
        this.applyFilter(filterValue);
      });
  }

  editQuestion(questionId: string): void {
    this.router.navigate(['/questions/edit', questionId]);
  }

  createQuestion(): void {
    this.router.navigate(['/questions/create']);
  }

  deleteQuestion(id: number) {
    this.questionsClient.delete(id).subscribe(_ => this.getQuestionList());
  }

  getQuestionList(): void {
    this.questionsClient.getAll(this.pageIndex + 1, this.pageSize, this.currentFilter).subscribe({
      next: data => {
        this.dataSource.data = data.questions!;
        this.totalCount = data.totalCount;
      },
      error: _ => {
        this.dataSource.data = [];
        this.totalCount = 0;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    if (this.pageSize != event.pageSize) {
      this.pageSize = event.pageSize;
      this.pageIndex = 0;
    } else {
      this.pageIndex = event.pageIndex;
    }
    this.getQuestionList();
  }

  applyFilter(filterValue: string | null): void {
    this.currentFilter = filterValue && filterValue.trim() === '' ? undefined : filterValue!.trim();
    this.pageIndex = 0;
    this.getQuestionList();
  }

  mapCategory(categoryNumber: number): string {
    switch (categoryNumber) {
      case 0:
        return 'Git';
      case 1:
        return 'Sql';
      case 2:
        return 'OOP';
      default:
        return 'Unknown';
    }
  }

  mapTypeButton(typeButtonNumber: number): string {
    switch (typeButtonNumber) {
      case 0:
        return 'Checkbox';
      case 1:
        return 'Radiobutton';
      default:
        return 'Unknown';
    }
  }
}
