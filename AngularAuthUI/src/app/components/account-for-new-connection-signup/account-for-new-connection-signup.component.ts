import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForms from 'src/app/helpers/validateForms';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-account-for-new-connection-signup',
  templateUrl: './account-for-new-connection-signup.component.html',
  styleUrls: ['./account-for-new-connection-signup.component.css']
})
export class AccountForNewConnectionSignupComponent {
  consumerRegistrationSignup!: FormGroup
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router, private toast: NgToastService) { }
  ngOnInit(): void {
    this.consumerRegistrationSignup = this.fb.group({
      consumerFullName: ['', Validators.required],
      consumerContractAccountNumber: [],
      consumerMobileNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      consumerAddress: [],
      consumerPassword: ['', Validators.required],
      consumerRetypePassword: ['', Validators.required]
    })
  }
  type: string = "password"
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash"
  hideShowPass() {
    this.isText = !this.isText
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash"
    this.isText ? this.type = "text" : this.type = "password"
  }
  
  onSubmittingConsumerRegistrationForm() {
    if (this.consumerRegistrationSignup.valid) {
      this.auth.onSubmittingConsumerRegistrationFormsignUp(this.consumerRegistrationSignup.value).subscribe({
        next: (res => {
          console.warn();
          var showConsumerMobileNumber = this.consumerRegistrationSignup.value['consumerMobileNumber'];
          this.consumerRegistrationSignup.reset();
          Swal.fire('SUCCESS', 'Your account has been successfully created!\n User ID: '+ showConsumerMobileNumber, 'success');
          this.router.navigate(['login']);
        }),
        error: ((err: any) => {
          this.toast.error({ detail: 'Warning', summary: "Something went wrong", duration: 5000 });
        })
      });
    } else {
      this.validateAllFormFields(this.consumerRegistrationSignup);
      if (!this.consumerRegistrationSignup.valid) {
        this.toast.warning({ detail: "WARNING", summary: "The form is Invalid !!!", duration: 4000 });
      }
    }
  }
  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field)
      if (control instanceof FormControl) {
        control.markAsDirty({ onlySelf: true })

      } else if (control instanceof FormGroup) {
        ValidateForms.validateAllFormFields(control)
      }
    })
  }
}
