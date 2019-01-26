import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { DataSharingService } from 'src/app/_services/data-sharing.service';
import { first } from 'rxjs/operators';
import { showToast } from 'src/app/toaster-helper';
import { WebsocketService } from 'src/app/_services/websocket.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css',
              '../../app.component.css',
            '../file/file.component.css']
})
export class UserComponent implements OnInit {

  public users = [];
  public errorMsg;
  
  isUserLoggedIn: boolean;

  constructor(private _usersService: UserService, private dataSharingService: DataSharingService, private _webSocketService: WebsocketService) { 
    this.dataSharingService.isUserLoggedIn.subscribe(
      value => {
        this.isUserLoggedIn = value;
      }
    )
  }

  ngOnInit() {
    this._usersService.getUsers()
        .subscribe(data => this.users = data,
                   error => this.errorMsg = error);
  }

  handleLogin(input_login, input_password){
    this._usersService.login(input_login, input_password).pipe(first()).subscribe(
      data => {},
      error => {
        showToast("An error: "+ error.name +" occurred while logging in, please try again!");
      },
      () => {
        if(localStorage.getItem('currentUser')==input_login){
          this.users.forEach(element => {
            if (element.user_id == localStorage.getItem('user_id')) {
              this._usersService.currentUserSubject = element;
            }
          });
          this._webSocketService.emitSocketClientTypeConnected(localStorage.getItem('user_id'));
          this.dataSharingService.isUserLoggedIn.next(true);
        }
      }
    );
    
  }
}
