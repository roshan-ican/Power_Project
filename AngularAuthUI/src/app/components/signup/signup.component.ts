import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForms from 'src/app/helpers/validateForms';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {

  signup_form!: FormGroup 
  constructor (private fb: FormBuilder, private auth: AuthService, private router: Router, private toast: NgToastService) {}

  ngOnInit(): void {
    this.signup_form = this.fb.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/)]],
      password: ['', Validators.required]
    });
  }
  



  type: string = "password"
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash"
  hideShowPass(){
    this.isText = !this.isText
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash"
    this.isText ? this.type = "text" : this.type = "password"
  }

  onSignUp() {
    // if the form is valid, send the object to the database
    if (this.signup_form.valid) {
      this.auth.signUp(this.signup_form.value).subscribe({
        next:(res=> {
          // alert(res.message)
          this.signup_form.reset()
          this.toast.success({detail:'SUCCESS', summary:res.message, duration: 5000})
          this.router.navigate(['login'])
        }),
        error:(err=> {
          // alert(err?.error.message)
          this.toast.error({detail: 'ERROR', summary: "Something went wrong", duration: 5000})
        })
      })
      console.log(this.signup_form.value);
    }
    // show an error using toastr and highlight required fields
    else {
      this.validateAllFormFields(this.signup_form);
      alert('invalid data entered')
    }
  }
  
  private validateAllFormFields(formGroup: FormGroup){
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field)
      if(control instanceof FormControl){
        control.markAsDirty({onlySelf: true})
        
      }else if(control instanceof FormGroup){
        ValidateForms.validateAllFormFields(control)
      }
    })
  }
  

}
