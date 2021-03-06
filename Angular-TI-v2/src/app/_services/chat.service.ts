import { Injectable } from '@angular/core';
import { WebsocketService } from './websocket.service';
import { Observable, Subject } from 'rxjs';
//import { map } from 'rxjs/add/operator/map';
import 'rxjs/Rx';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  messages: Subject<any>;

  //constructor() {}
  constructor(private wsService: WebsocketService) {
    this.messages = <Subject<any>> wsService
      .connect()
      .pipe(map((response: any): any => {
        return response;
      }))
   }

   sendMessage(username, message) {
     this.messages.next({username, message});
    //this.wsService.emitEventOnChatMessageSent(username, message);
   }
}
