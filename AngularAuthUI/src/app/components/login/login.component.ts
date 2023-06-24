import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForms from 'src/app/helpers/validateForms';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginForm!: FormGroup;

  constructor(private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService
  ) { }

  ngOnInit(): void {
    // Initialize the loginForm FormGroup with form controls and validators
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";

  // Toggle between showing and hiding the password
  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  // Handle the login button click event
  onLogin() {
    // If the form is valid, send the loginForm value to the server
    if (this.loginForm.valid) {
      console.log(this.loginForm.value);
      this.auth.login(this.loginForm.value).subscribe({
        next: (res) => {
          // alert(res.message);
          this.loginForm.reset();
          this.auth.storeToken(res.token)
          this.toast.success({detail: "SUCCESS", summary:res.message, duration: 4000})
          this.router.navigate(['dashboard'])
        },
        error: (err) => {
          // alert(err?.error.message);
          this.toast.error({detail: "ERROR", summary: "Something went wrong", duration: 4000})
          console.log(err);
        }
      });
    }
    // If the form is invalid, show an error message and mark required fields as dirty
    else {
      this.validateAllFormFields(this.loginForm);
      alert('The form is invalid');
    }
  }

  // Recursively mark all form fields as dirty
  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsDirty({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        ValidateForms.validateAllFormFields(control);
      }
    });
  }
}
