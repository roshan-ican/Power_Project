import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl:string = "https://localhost:44378/api/User/"
  private secondbaseUrl : string = "https://localhost:44378/api/ConsumerRegistrationUsers/"

  constructor(private http: HttpClient) { }

  getUsers() {
    return this.http.get<any>(this.baseUrl)
  }
  getCustomerUsers() {
    return this.http.get<any>(this.secondbaseUrl)
  }
}
