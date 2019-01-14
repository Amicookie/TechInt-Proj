import { Component, OnInit } from '@angular/core';
import { ChatService } from './_services/chat.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Angular-TI-v2';

  constructor(private chat: ChatService) {}

  ngOnInit(){
    this.chat.messages.subscribe(message => {
      console.log(message);
    })
  }

  sendMessage() {
    this.chat.sendMessage("username","Test Message");
  }

}
