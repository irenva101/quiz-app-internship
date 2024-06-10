import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CandidateClient, RegisterCandidateCommand } from '../api/api-reference';
import { Router } from '@angular/router';
import { Observable, catchError, map, of, pipe } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email,Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$") ]),
    firstname: new FormControl('', Validators.required),
    lastname: new FormControl('', Validators.required),
    faculty: new FormControl('', Validators.required),
    course: new FormControl('', Validators.required)
  });

  constructor(
    private router: Router,
    private candidateClient: CandidateClient
  ) { }

  onSubmit() {
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;

      const command = new RegisterCandidateCommand({
        email: formData.email!,
        firstName: formData.firstname!,
        lastName: formData.lastname!,
        faculty: formData.faculty!,
        course: formData.course!
      });
      this.candidateClient.registerCandidate(command).subscribe({
        next: (response: any) => {
          const candidateId = response.candidateId
          this.registerForm.reset();
          this.router.navigate(['/test-page'], { queryParams: { candidateId } })
        },
        error: () => {
          console.log('Error happened');
          this.registerForm.get('email')?.setErrors({ emailTaken: true })
        }
      });
    }
  }
}