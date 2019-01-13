import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {

  public users = [];
  public errorMsg;
  constructor(private _usersService: UserService) { }

  ngOnInit() {
    this._usersService.getUsers()
        .subscribe(data => this.users = data,
                   error => this.errorMsg = error);
  }

  checkPassword() {
    //sprawdź czy PW jest ok, żeby zalogować :)
  }

  handleLogin(input_login, input_password){
    this._usersService.handleLogin(input_login, input_password);
  }
}