import { Component, OnInit } from '@angular/core';
import { DataSharingService } from 'src/app/_services/data-sharing.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css',
  '../../app.component.css', '../file/file.component.css']
})
export class LoginComponent implements OnInit {

  isUserLoggedIn: boolean;
 

  constructor(private dataSharingService: DataSharingService) { 
    this.dataSharingService.isUserLoggedIn.subscribe(
      value => {
        this.isUserLoggedIn = value;
      }
    )
  }

  ngOnInit() {
  }

}
