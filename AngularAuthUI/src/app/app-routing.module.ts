import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuard } from './guards/auth.guard';
import { AccountForNewConnectionSignupComponent } from './components/account-for-new-connection-signup/account-for-new-connection-signup.component';
import { AccountCreateSignupComponent } from './components/account-create-signup/account-create-signup.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard]},
  // added new component by mukesh
  {
    path: 'account-for-new-connection-signup', component: AccountForNewConnectionSignupComponent
  },
  {
    path: 'account-create-signup', component: AccountCreateSignupComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
