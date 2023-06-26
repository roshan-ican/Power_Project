import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserStoreService {

  private fullName$ = new BehaviorSubject<string>("");
  private role$ = new BehaviorSubject<string>("");

  private consumerUserId$ = new BehaviorSubject<string>("");
  private consumerRole$ = new BehaviorSubject<string>("");

  public getConsumerRoleFromStore()
  {
    return this.consumerRole$.asObservable();
  }
  public setConsumerRoleForStore (role: any)
  {
    this.consumerRole$.next(role);
  }
  public getConsumerUserIdFromStore()
  {
    return this.consumerUserId$.asObservable();
  }
  public setConsumerUserIdFromStore(consumerUserId: any)
  {
    return this.consumerUserId$.next(consumerUserId)
  }

  constructor() { }

  public getRoleFromStore(){
    return this.role$.asObservable();
  }
  public setRoleForStore(role: string){
    this.role$.next(role)
  }
  public getFullNameFromStore(){
    return this.fullName$.asObservable();
  }
  public setFullNameForStore(fullname:string) {
    this.fullName$.next(fullname)
  }
}
