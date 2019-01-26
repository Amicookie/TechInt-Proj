import { Component, OnInit } from '@angular/core';
import { DataSharingService } from 'src/app/_services/data-sharing.service';
import { WebsocketService } from 'src/app/_services/websocket.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css',
  '../../app.component.css', '../file/file.component.css']
})
export class LoginComponent implements OnInit {

  isUserLoggedIn: boolean;
 

  constructor(private dataSharingService: DataSharingService, private _webSocketService: WebsocketService) { 
    this.dataSharingService.isUserLoggedIn.subscribe(
      value => {
        this.isUserLoggedIn = value;
        if(value == true) {
          this._webSocketService.emitSocketClientTypeConnected(localStorage.getItem('user_id'));
        }
      }
    )
  }

  ngOnInit() {
  }

}
