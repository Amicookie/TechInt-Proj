import { Component, OnInit } from '@angular/core';
import { DataSharingService } from 'src/app/_services/data-sharing.service';
import { UserService } from 'src/app/_services/user.service';
import { showToast } from 'src/app/toaster-helper';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css',
              '../../../app.component.css']
})
export class HeaderComponent implements OnInit {

  isUserLoggedIn: boolean;
  user_name: string;

  constructor(private dataSharingService: DataSharingService, private userService: UserService) { 
    this.dataSharingService.isUserLoggedIn.subscribe(
      value => {
        this.isUserLoggedIn = value;
      }
    );
  }

  ngOnInit() {
  }

  handleLogout(){
    this.userService.logout();
    this.dataSharingService.isUserLoggedIn.next(false);
  }

}
