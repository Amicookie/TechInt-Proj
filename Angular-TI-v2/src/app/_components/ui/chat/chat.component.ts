import { Component, OnInit } from '@angular/core';
import { ChatService } from '../../../_services/chat.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  message_input:string = null;
  constructor(private chatService: ChatService, private userService: UserService) { }

  LoggedInUsers = this.userService.usersCurrentlyLoggedIn;

  ngOnInit() {

  }

  openForm() {
    document.getElementById("myForm").style.display = "block";
  }
  
  closeForm() {
    document.getElementById("myForm").style.display = "none";
  }

  sendMessage(message){
    this.chatService.sendMessage(localStorage.getItem('currentUser'), message);
    this.message_input = '';
  }
}
