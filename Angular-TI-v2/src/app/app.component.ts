import { Component } from '@angular/core';
import {ChatService} from './_services/chat.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Angular-TI-v2';

  constructor(private chat: ChatService) {}

  ngOnInit(){
    this.chat.messages.subscribe(msg => {
      console.log(msg);
    })
  }

  sendMessage() {
    this.chat.sendMsg("test message");
  }
}
