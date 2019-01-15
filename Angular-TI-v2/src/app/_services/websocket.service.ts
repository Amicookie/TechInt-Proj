import { Injectable } from '@angular/core';
import * as io from 'socket.io-client';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
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
  //public _file_locked = false;

  public _file_locked: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public _locked_file_id: BehaviorSubject<number> = new BehaviorSubject<number>(null);;

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

      this.socket.on('fileSaved', (data)=> {
        showToast('A File \"'+ data.file_name + '\" has just been shared by ' + data.username + '!');
        // dodawanie plików do listy
      })

      this.socket.on('fileUpdated', (data)=>{
        showToast('File \"'+ data.file_name +'\" updated by '+ data.username+'!');
        // edytowanie listy plików
      })

      this.socket.on('fileLocked', (data)=>{
        showToast('File \"'+ data.file_name +'\" locked by '+ data.username+'!');
        // disable checkbox!!!
        if(localStorage.getItem('user_id') == data.user_id) {
          this._file_locked.next(false);
        } else {
          this._locked_file_id.next(data.file_id);
          this._file_locked.next(true);
        }
      })

      this.socket.on('fileUnlocked', (data)=>{
        showToast('File \"'+ data.file_name +'\" unlocked!');
        // enable checkbox!!!
        this._locked_file_id.next(null);
        this._file_locked.next(false);
      })

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

  emitEventOnFileLocked(user_id, username, file_name, file_id){
    this.socket.emit('fileLocked', JSON.stringify({user_id, username, file_name, file_id}));
  }

  
  emitEventOnFileUnlocked(username, file_name){
    this.socket.emit('fileUnlocked', JSON.stringify({username, file_name}));
  }

  emitEventOnFileUpdated(file_name, username){
    this.socket.emit('fileUpdated', JSON.stringify({file_name, username}));
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