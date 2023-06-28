import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountForNewConnectionSignupComponent } from './account-for-new-connection-signup.component';

describe('AccountForNewConnectionSignupComponent', () => {
  let component: AccountForNewConnectionSignupComponent;
  let fixture: ComponentFixture<AccountForNewConnectionSignupComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AccountForNewConnectionSignupComponent]
    });
    fixture = TestBed.createComponent(AccountForNewConnectionSignupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
