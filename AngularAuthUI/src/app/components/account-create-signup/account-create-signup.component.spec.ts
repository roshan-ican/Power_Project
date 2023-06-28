import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCreateSignupComponent } from './account-create-signup.component';

describe('AccountCreateSignupComponent', () => {
  let component: AccountCreateSignupComponent;
  let fixture: ComponentFixture<AccountCreateSignupComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AccountCreateSignupComponent]
    });
    fixture = TestBed.createComponent(AccountCreateSignupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
