import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http"
import { Router } from '@angular/router';
import {JwtHelperService} from "@auth0/angular-jwt"
import { TokenApiModel } from '../models/token-api.model';


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
  onSubmittingConsumerCreateAccountFormsignUp(ConsumerRegistrationForms:any) {
    return this.http.post<any>(`${this.secondbaseUrl}Consumer-Registration-User-second`, ConsumerRegistrationForms)
  }
  onSubmittingConsumerRegistrationFormsLogin(ConsumerRegistrationFormsloginObj: any[])
  {
    return this.http.post<any>(`${this.secondbaseUrl}Consumer-Registration-User-Authenticate`, ConsumerRegistrationFormsloginObj)
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
  storeToken(accessToken: string): void {
    localStorage.setItem('accessToken', accessToken);
  }
  storeRefreshToken(tokenValue:string){
    localStorage.setItem('refreshToken', tokenValue)
  }
  getToken(){
    return localStorage.getItem('accessToken')
  }
  getRefreshToken() {
    localStorage.getItem('refreshToken')
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('accessToken')
  }
  decodedToken() {
    // const jwtHelper = new JwtHelperService()
    // const token = this.getToken()!;
    // return  jwtHelper.decodeToken(token)

    const jwtHelper = new JwtHelperService()
    const token = this.getToken()!;
    console.log(token)
    console.log(jwtHelper.decodeToken(token));
    return  jwtHelper.decodeToken(token)
  }
  getfullnameFromToken() {
    if(this.userPayload)
    console.log(this.userPayload, 'MAAL');
    return this.userPayload.name;
  }

  getRoleFromToken(){
    if(this.userPayload)
    return this.userPayload.role;
  }

  renewToken(tokenApi: TokenApiModel){
    return this.http.post<any>(`${this.baseUrl}refresh`, tokenApi)
  }

}
