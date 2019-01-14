import { Injectable } from '@angular/core';
import * as io from 'socket.io-client';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { IFile } from '../_models/file';
import { IUser } from '../_models/user';
import { showToast } from '../toaster-helper';
import { Socket } from 'ng6-socket-io';

@Injectable({
  providedIn: 'root'
})

class ChatService {
 
  constructor(private socket: Socket) { }

  sendMessage(username: string, message: string){
      this.socket.emit("chat", {username, message});
  }

  getMessage() {
      return this.socket
          .fromEvent<any>("chat")
          .map(data => {data.username, data.message});
  }
  
  close() {
    this.socket.disconnect()
  }
}

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {

  private socket; // socket connects to socket.io server

  constructor() {
    //this.socket = io.connect(environment.ws_url);
  }

  // connect() {
  //   this.socket = io.connect(environment.ws_url);
  // }

  close() {
    this.socket.disconnect();
  }

  connect(): Subject<MessageEvent>{
    this.socket = io.connect(environment.ws_url);

    let observable = new Observable(observer => {

      this.socket.on('chat', (data)=> {
        console.log("Received message from chat");
        var chatContainer = document.getElementById("chat-container");
        chatContainer.innerHTML += '<p><strong>'+data.username+':</strong> '+data.message+'</p>';
        document.getElementById("msg_input").innerHTML = "";  
        observer.next(data);
      })

      // this.socket.on('fileSaved', (data)=> {
      //   console.log("Received a msg from server");
      //   showToast('A File \"'+ data.file_name + '\" has just been shared by ' + data.username + '!');
      //   observer.next(data);
      // })

      return() => {
        this.socket.disconnect();
      }
    })

    let observer = {
      next: (data: Object) => {
        this.socket.emit('chat', JSON.stringify(data));
      },
    }
    return Subject.create(observer,observable);
  }


   //obsługa eventów

  emitEventOnChatMessageSent(username, message){

    //this.socket.emit('chat', {username, message});
  }

  emitEventOnFileSaved(username, file_name) {
    this.socket.emit('fileSaved', JSON.stringify({username, file_name}));
  }

  emitEventOnFileLocked(fileLocked){
    this.socket.emit('fileLocked', JSON.stringify(fileLocked));
  }

  emitEventOnFileUpdated(file_name, user_login){
    this.socket.emit('fileUpdated', JSON.stringify({file_name, user_login}));
  }

  // consume File Saved ---- create new file
  consumeEventOnChatMessageSent() {
    this.socket.on('chat', function(data){
      var chatContainer = document.getElementById("chat-container");
      chatContainer.innerHTML += '<div class="container"><p><strong>'+data.username+'</strong>'+data.message+'</p><span class="time-right">'+new Date().toLocaleTimeString()+'</span></div>'
    });
  }


  consumeEventOnFileSaved(){
    this.socket.on('fileSaved', function(data){
      showToast('A File \"'+ data.file_name + '\" has just been shared by ' + data.username + '!');
    });
  }

  consumeEventOnFileLocked(){
    this.socket.on('fileLocked', function(data){
      showToast('File \"'+ data.file_name +'\" locked by '+ data.username+'!');
      });
  }

  consumeEventOnFileUpdated(){
    this.socket.on('fileUpdated', function(data){
      showToast('File \"'+ data.file_name +'\" updated by '+ data.username+'!');
    });
  }

}