import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForms from 'src/app/helpers/validateForms';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginForm!: FormGroup;
  consumerRegistrationlogin!: FormGroup;

  // RESET PASSWORD


  constructor(private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService,
    private userStore: UserStoreService
  ) { }

  ngOnInit(): void {
    // Initialize the loginForm FormGroup with form controls and validators
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    this.consumerRegistrationlogin = this.fb.group({
      ConsumerMobileNumber: ['', Validators.required],
      ConsumerPassword: ['', Validators.required]
    })
    
  }

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";
  public resetPasswordNumber!:string;
  public isValidNumber!:boolean;


  // Toggle between showing and hiding the password
  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  // Handle the login button click event
  onConsumerRegistratedUserLogin() {
    if (this.consumerRegistrationlogin.valid) {
      this.auth.onSubmittingConsumerRegistrationFormsLogin(this.consumerRegistrationlogin.value).subscribe({
        next: (res) => {
          this.consumerRegistrationlogin.reset();
          this.auth.storeToken(res.accessToken)
          this.auth.storeRefreshToken(res.refreshToken)
          const tokenPayload = this.auth.decodedToken();
          this.userStore.setConsumerUserIdFromStore(tokenPayload.ConsumerMobileNumber)
          this.userStore.setConsumerRoleForStore(tokenPayload.ConsumerRole)
          this.toast.success({ detail: "SUCCESS", summary: res.message, duration: 4000 })
          this.router.navigate(['dashboard'])
        },
        error: (err) => {
          this.toast.error({ detail: "ERROR", summary: "Something went wrong", duration: 4000 })
        }
      })
    }
    else {
      this.validateAllFormFields(this.consumerRegistrationlogin);
      if (!this.consumerRegistrationlogin.valid) {
        this.toast.warning({ detail: "WARNING", summary: "The form is Invalid !!!", duration: 4000 })
      }
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

  //onclicksendverification code on consumer Registred mobile number
  // onConsumerSubmittingSendVerificationCode() {
  //   if (this.consumerRegistrationlogin.valid) {
  //     this.auth.onSubmittingConsumerSendVerificationCode(this.consumerRegistrationlogin.value).subscribe({
  //       next: (res: any[]) => {
  //         console.warn(res);
  //         this.consumerRegistrationlogin.reset();
  //       },
  //       error: (err: any[]) => {
  //         this.toast.error({ detail: "ERROR", summary: "Something went wrong", duration: 4000 })
  //       }
  //     })
  //   }
  //   else {
  //     this.validateAllFormFields(this.consumerRegistrationlogin);
  //     if (!this.consumerRegistrationlogin.valid) {
  //       this.toast.warning({ detail: "WARNING", summary: "The form is Invalid !!!", duration: 4000 })
  //     }
  //   }
  // }

checkValidPhoneNumber(event: string) {
  // Perform phone number validation logic
  const phoneNumber = event.trim(); // Remove leading/trailing whitespace
  const phoneNumberPattern = /^\d{10}$/; // Assumes a 10-digit phone number
  this.isValidNumber = phoneNumberPattern.test(phoneNumber);
  // Output validation result
  return this.isValidNumber
}

confirmToSend(){
  if(this.checkValidPhoneNumber(this.resetPasswordNumber)){
    console.log(this.resetPasswordNumber);
    this.resetPasswordNumber = "";
    const buttonRef = document.getElementById("closeBtn")
    buttonRef?.click()
    // API CALL NEED TO BE DONE
  }
} 

}
