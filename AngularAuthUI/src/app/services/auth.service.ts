import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http"
import { Router } from '@angular/router';
import {JwtHelperService} from "@auth0/angular-jwt"


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl:string = "https://localhost:44378/api/User/"
  private secondbaseUrl : string = "https://localhost:44378/api/ConsumerRegistrationUsers/"

  private userPayload: any;

  constructor(private http: HttpClient, private router: Router) {
    this.userPayload = this.decodedToken();
   }

   onSubmittingConsumerRegistrationFormsignUp(ConsumerRegistrationForms:any) {
    return this.http.post<any>(`${this.secondbaseUrl}Consumer-Registration-User`, ConsumerRegistrationForms)
  }


  signUp(userObj:any) {
    return this.http.post<any>(`${this.baseUrl}register`, userObj)
  }

  login(loginObj: any) {
    return this.http.post<any>(`${this.baseUrl}authenticate`, loginObj)
  }
  signOut() {
    localStorage.clear()
    this.router.navigate(['login'])

  }
  storeToken(tokenValue: string) {
    localStorage.setItem('token', tokenValue)
  }
  getToken(){
    return localStorage.getItem('token')
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token')
  }
  decodedToken() {
    const jwtHelper = new JwtHelperService()
    const token = this.getToken()!;
    console.log(jwtHelper.decodeToken(token));
    return  jwtHelper.decodeToken(token)
  }
  getfullnameFromToken() {
    if(this.userPayload)
    return this.userPayload.name;
  }

  getRoleFromToken(){
    if(this.userPayload)
    return this.userPayload.role;
  }

}
