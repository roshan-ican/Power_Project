import { Component, OnInit } from '@angular/core';
import { navItems } from '../nav';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-menu-sidebar',
  templateUrl: './menu-sidebar.component.html',
  styleUrls: ['./menu-sidebar.component.css']
})
export class MenuSidebarComponent {
  collapseShow = "hidden";
  navItems: Array<any> = [];


  constructor(private authService: AuthService)
  {
    this.navItems = navItems;
  }

  ngOnInit() {
    console.log(this.navItems, 'NAV ITEMS')
  }


  toggleCollapseShow(classes: any) {
    this.collapseShow = classes;
  }
  logout() {
    this.authService.signOut();
  }


}
